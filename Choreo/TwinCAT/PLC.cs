using NLog.LayoutRenderers.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using TwinCAT;
using TwinCAT.Ads;
using TwinCAT.Ads.Reactive;
using TwinCAT.Ads.SumCommand;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;
using static Choreo.Globals;
using Cfg = Choreo.Configuration;

namespace Choreo.TwinCAT {
    public interface IPlc {
        event EventHandler SymbolsUpdated;
        void Upload<T>(T obj);
        void Download<T>(T obj);
        bool SaveGroupMotors(int groupIndex, ushort bitmap);
        bool GetMotorsGroup(ref ushort[] motorGroups);
        bool ClearMotionAndJog();
        void Jog(Axis axis, int direction);
        void Calibrate(Axis axis);
    }

    static public class PlcFactory
    {
        public static IPlc New() => new AdsPlc(Cfg.PlcAmsNetId, Cfg.PlcAmsPort).Init();
    }
    
    class AdsPlc: PropertyChangedNotifier, IPlc
    {
        AdsSession adsSession;
        IDisposable notificationSubscription;
        IObserver<ValueChangedArgs> symbolNotificationObserver;
        Timer monitorTimer;
        TagCollection tags;
        public event EventHandler SymbolsUpdated;

        public AdsPlc(AmsNetId amsNetId, AmsPort amsPort) {
            adsSession = new AdsSession(amsNetId, (int)amsPort);
        }

        AdsConnection Connection => adsSession.Connection;
        public AdsPlc Init()
        {
            adsSession.Connect();
            adsSession.ConnectionStateChanged += Session_ConnectionStateChanged;
            tags = new TagCollection();
            allEnabs.AddRange(VM.Axes.SelectMany(ax => enabProps.Select(prop => tags[ax, prop])));
            symbolNotificationObserver = Observer.Create<ValueChangedArgs>(OnSymbolNotification);
            StartMonitor();
            return this;
        }


        bool isConnectionOK;
        bool IsConnectionOK {
            get => isConnectionOK;
            set { isConnectionOK = value; Notify()("IsOn"); }
        }

        bool areSymbolsOK;
        bool AreSymbolsOK {
            get => areSymbolsOK;
            set { areSymbolsOK = value; Notify()("IsOn"); }
        }

        private bool isWatchdogOK;
        public bool IsWatchdogOK {
            get { return isWatchdogOK; }
            set { isWatchdogOK = value; Notify()("IsOn"); }
        }


        public bool IsOn => IsWatchdogOK;

        #region Monitor
        void StartMonitor() {
            monitorTimer = new Timer(Monitor, null, 1000, Timeout.Infinite);
        }

        int watchdogCounter = int.MaxValue;
        void Monitor(object state) {
            monitorTimer.Change(Timeout.Infinite, Timeout.Infinite);

            bool WatchdogOK() {
                return Log.ExOnce(() => {
                    try {
                        var watchdog = (bool)Connection.ReadSymbol("GVL.Watchdog_OK", typeof(bool), false);
                        if (watchdog) {
                            Connection.WriteSymbol("GVL.Watchdog_OK", false, false);
                            watchdogCounter = 0;
                            return true;
                        }
                        else {
                            if (watchdogCounter >= 5) return false;
                            ++watchdogCounter;
                        }
                        return true;
                    }
                    catch {
                        watchdogCounter = int.MaxValue;
                        throw;
                    }
                }
                , "Unhealthy PLC Connection");
            }

            if (IsConnectionOK && Connection.State != ConnectionState.Connected) {
                Unhook();
                IsConnectionOK = false;
            }
            else
            if (!IsConnectionOK && Connection.State == ConnectionState.Connected) {
                try {
                    Hook();
                    IsConnectionOK = true;
                }
                catch { }
            }

            if (IsConnectionOK && !AreSymbolsOK) SetupSymbolsAndNotifications();

            IsWatchdogOK = IsConnectionOK && AreSymbolsOK && WatchdogOK();

            monitorTimer.Change(1000, Timeout.Infinite);
        }

        void Unhook() {
            Connection.AdsStateChanged -= Conn_AdsStateChanged;
            Connection.AdsNotificationError -= Conn_AdsNotificationError;
            Connection.AdsSymbolVersionChanged -= Conn_AdsSymbolVersionChanged;
        }

        void Hook() {
            Connection.AdsStateChanged += Conn_AdsStateChanged;
            Connection.AdsNotificationError += Conn_AdsNotificationError;
            Connection.AdsSymbolVersionChanged += Conn_AdsSymbolVersionChanged;
        }

        #endregion

        #region Notifications
        void SetupSymbolsAndNotifications() {
            Log.ExOnce(() => {
                notificationSubscription?.Dispose();
                tags.Symbols = SymbolLoaderFactory.Create(Connection, SymbolLoaderSettings.Default).Symbols;
                SymbolCollection notificationSymbols = new SymbolCollection(tags.Select(tag => tag.Symbol));
                notificationSubscription = notificationSymbols.WhenValueChangedAnnotated().Subscribe(symbolNotificationObserver);
                AreSymbolsOK = true;
                SymbolsUpdated?.Invoke(this, null);
            }, "PLC Symbols do not match");
        }
        void OnSymbolNotification(ValueChangedArgs vca) => tags[vca.Symbol].Push(vca.Value);
        #endregion

        #region Events
        void Conn_AdsSymbolVersionChanged(object sender, EventArgs e) => AreSymbolsOK = false;

        void Conn_AdsNotificationError(object sender, AdsNotificationErrorEventArgs e)
        {
        }

        void Conn_AdsStateChanged(object sender, AdsStateChangedEventArgs e)
        {
        }

        void Session_ConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e) 
        { 
        }
        #endregion

        #region IPlc Implementation
        public void Upload<T>(T obj) {
            if (!IsOn) return;
            GetType().GetMethod("Upload",
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new Type[] { typeof(T) },
                null)
                .Invoke(this, new object[] { obj });
        }

        public void Download<T>(T obj) {
            if (!IsOn) return;
            GetType().GetMethod("Download",
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new Type[] { typeof(T) },
                null)
                .Invoke(this, new object[] { obj });
        }

        void Upload(Axis axis) {
            var values = new object[] {
                axis.MinLoad
                , axis.MaxLoad
                , axis.LoadOffs
                , axis.MinVel
                , axis.MaxVel
                , axis.SoftDnRotations
                , axis.SoftUpRotations
                , axis.UserEnable
            };

            List<ISymbol> valueSyms = new List<ISymbol>();
            valueSyms.AddRange(
                new string[] {
                    nameof(Axis.MinLoad)
                    , nameof(Axis.MaxLoad)
                    , nameof(Axis.LoadOffs)
                    , nameof(Axis.MinVel)
                    , nameof(Axis.MaxVel)
                    , nameof(Axis.SoftDnRotations)
                    , nameof(Axis.SoftUpRotations)
                    , nameof(Axis.UserEnable)
                    }
                .Select(propName => tags[axis, propName].Symbol));

            new SumSymbolWrite(Connection, valueSyms).Write(values);
            Connection.WriteSymbol(tags.PathOf(VM, nameof(ViewModel.ParameterWrite)), true, false);
        }

        void Download(Axis axis) {
            List<ISymbol> valueSyms = new List<ISymbol>();
            valueSyms.AddRange(
                new string[] {
                    nameof(Axis.MinLoad)
                    , nameof(Axis.MaxLoad)
                    , nameof(Axis.LoadOffs)
                    , nameof(Axis.MinVel)
                    , nameof(Axis.MaxVel)
                    , nameof(Axis.SoftDnRotations)
                    , nameof(Axis.SoftUpRotations)
                    , nameof(Axis.UserEnable)
                    }
                .Select(propName => tags[axis, propName].Symbol));

            var values = new SumSymbolRead(Connection, valueSyms).Read();
            for (int i = 0; i < valueSyms.Count; i++) tags[valueSyms[i]].Push(values[i]);
        }

        static readonly string[] moveProps = new string[] { nameof(Axis.MoveVal), nameof(Axis.Velocity), nameof(Axis.Accel), nameof(Axis.Decel) };
        static readonly string[] enabProps = new string[] { nameof(Axis.MAEnable), nameof(Axis.MREnable), nameof(Axis.JogUpEnable), nameof(Axis.JogDnEnable) };
        static readonly List<ITag> allEnabs = new List<ITag>();
        static readonly object[] allEnabValues = Enumerable.Repeat(false, 96).OfType<object>().ToArray();
        void Upload(Motion motion) {
            var values = new object[] {
                0.0
                , motion.Velocity
                , motion.Acceleration
                , motion.Deceleration
            };

            var enabSyms = new List<ISymbol>();
            var valueSyms = new List<ISymbol>();
            var enaProps = new string[] { 
                motion.Relative ? nameof(Axis.MREnable): nameof(Axis.MAEnable)
                , motion.Relative ? nameof(Axis.MAEnable): nameof(Axis.MREnable)
                , nameof(Axis.JogUpEnable)
                , nameof(Axis.JogDnEnable) 
            };
            var move = motion.Relative ? motion.RelativeSetpoint : motion.AbsoluteSetpoint;

            foreach (var ax in VM.Axes) {
                if (!motion.Contains(ax)) continue;

                valueSyms.Clear();
                valueSyms.AddRange(moveProps.Select(prop => tags[ax, prop].Symbol));
                values[0] = move * ax.RotationsPerFoot;
                new SumSymbolWrite(Connection, valueSyms).Write(values);

                enabSyms.AddRange(enaProps.Select(p => tags[ax, p].Symbol));
            }

            var tf = new object[] { true, false, false, false };
            new SumSymbolWrite(Connection, enabSyms).Write(Enumerable.Repeat(tf, enabSyms.Count / 4).SelectMany(tfe => tfe).ToArray());
        }

        void Upload(double jogVel) {
            var path = tags.PathOf(VM, nameof(ViewModel.JogVelocity));
            Connection.WriteSymbol(path, jogVel * 100.0, false);
        }

        void Upload(Preset preset) {
            var enabSyms = new List<ISymbol>();
            var valueSyms = new List<ISymbol>();

            var aps = new List<(Axis, double)>();
            aps.AddRange(preset.MotorPositions.Select(mp => (VM.Motors[mp.Key] as Axis, mp.Value)));
            aps.AddRange(preset.GroupPositions.Select(gp => (VM.Groups[gp.Key] as Axis, gp.Value)));

            foreach ((Axis ax, double pos) in aps) {
                var values = new object[] {
                    pos * ax.RotationsPerFoot
                    , ax.DefVel
                    , ax.DefAcc
                    , ax.DefDec
                };

                valueSyms.Clear();
                valueSyms.AddRange(moveProps.Select(prop => tags[ax, prop].Symbol));
                new SumSymbolWrite(Connection, valueSyms).Write(values);

                enabSyms.AddRange(enabProps.Select(p => tags[ax, p].Symbol));
            }

            var tf = new object[] { true, false, false, false };
            new SumSymbolWrite(Connection, enabSyms).Write(Enumerable.Repeat(tf, enabSyms.Count / 4).SelectMany(tfe => tfe).ToArray());
        }

        static readonly string[] cueProps = new string[] { nameof(ViewModel.CueLoaded), nameof(ViewModel.CueComplete)};
        void Upload(Cue cue) {
            var valueSyms = new List<ISymbol>();
            var values = new List<object>();
            var enabValues = (object[])allEnabValues.Clone();

            if (cue is Cue) {
                foreach (var row in cue.Rows) {
                    var rowValues = new object[] {
                    0.0
                    , row.Velocity
                    , row.Acceleration
                    , row.Deceleration
                };

                    foreach (var motor in VM.Motors.Where(motor => row.Motors[motor.Index])) {
                        valueSyms.AddRange(moveProps.Select(prop => tags[motor, prop].Symbol));
                        rowValues[0] = row.Target * motor.RotationsPerFoot;
                        values.AddRange(rowValues);
                        enabValues[motor.Index * 4] = true;
                    }

                    foreach (var group in VM.Groups.Where(group => row.Groups[group.Index])) {
                        valueSyms.AddRange(moveProps.Select(prop => tags[group, prop].Symbol));
                        rowValues[0] = row.Target * group.RotationsPerFoot;
                        values.AddRange(rowValues);
                        enabValues[(group.Index + 16) * 4] = true;
                    }
                }

                new SumSymbolWrite(Connection, valueSyms).Write(values.ToArray());
                new SumSymbolWrite(Connection, allEnabs.Select(tag => tag.Symbol).ToList()).Write(enabValues);
                new SumSymbolWrite(Connection, cueProps.Select(prop => tags[VM, prop].Symbol).ToList()).Write(new object[] { true, false });
                VM.LoadedCue = cue.Number;
            }
            else {
                new SumSymbolWrite(Connection, allEnabs.Select(tag => tag.Symbol).ToList()).Write(enabValues);
                new SumSymbolWrite(Connection, cueProps.Select(prop => tags[VM, prop].Symbol).ToList()).Write(new object[] { false, false });
                VM.LoadedCue = 0;
            }
        }

        public bool SaveGroupMotors(int groupIndex, ushort bitmap) => PlcMethod()?.Invoke(groupIndex, bitmap) == 0;

        public bool GetMotorsGroup(ref ushort[] motorGroups) => PlcMethod(motorGroups)?.Invoke(out motorGroups) == 0;

        delegate int PlcMethodDelegate(params object[] @params);
        delegate int PlcMethodOutDelegate<T>(out T output, params object[] @params);
        PlcMethodDelegate PlcMethod([CallerMemberName]string method = null) {
            if (!IsOn) return null;
            int fun(params object[] @params) {
                var success = Connection.TryInvokeRpcMethod("GVL.UI", method, @params, out var res) == AdsErrorCode.NoError;
                return (int)res;
            }
            return IsOn ? fun : new PlcMethodDelegate((_) => -1);
        }

        PlcMethodOutDelegate<T> PlcMethod<T>(T outObj = null, [CallerMemberName]string method = null) where T:class {
            if (!IsOn) return null;
            var outSpecs = new AnyTypeSpecifier[] { new AnyTypeSpecifier(outObj) };
            int fun<U>(out U output, params object[] @params) {
                var success = Connection.TryInvokeRpcMethod("GVL.UI", method, @params, outSpecs, null, out var @out, out var res) == AdsErrorCode.NoError;
                output = (U)@out[0];
                return (int)res;
            }
            return IsOn ? fun : new PlcMethodOutDelegate<T>((out T _1, object[] _2) => { _1 = null; return -1; });
        }

        public void Jog(Axis axis, int direction) {
            if (!IsOn) return;

            var values = new object[] {
                direction > 0
                , direction < 0
                , false
                , false
            };

            List<ISymbol> valueSyms = new List<ISymbol>();
            valueSyms.AddRange(
                new string[] {
                    nameof(Axis.JogUpEnable)
                    , nameof(Axis.JogDnEnable)
                    , nameof(Axis.MAEnable)
                    , nameof(Axis.MREnable)
                }
                .Select(propName => tags[axis, propName].Symbol));

            new SumSymbolWrite(Connection, valueSyms).Write(values);
        }

        public bool ClearMotionAndJog() => PlcMethod()?.Invoke() == 0;

        public void Calibrate(Axis axis) {
            if (!IsOn) return;

            var pathVal = tags.PathOf(axis, nameof(Axis.CalibrationRotations));
            var pathSet = tags.PathOf(axis, nameof(Axis.CalibrationSave));
            Connection.WriteSymbol(pathVal, axis.CalibrationRotations, false);
            Connection.WriteSymbol(pathSet, true, false);
        }

        #endregion
    }

    [System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    sealed class PlcAttribute : Attribute {
        public PlcAttribute(string path = null) {
            Path = path;
        }

        public string Path { get; private set; }
    }

}
