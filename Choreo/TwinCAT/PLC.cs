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
            symbolNotificationObserver = Observer.Create<ValueChangedArgs>(OnSymbolNotification);
            StartMonitor();
            return this;
        }


        bool isConnectionOK;
        bool IsConnectionOK {
            get => isConnectionOK;
            set { isConnectionOK = value; OnPropertyChanged("IsOn"); }
        }

        bool areSymbolsOK;
        bool AreSymbolsOK {
            get => areSymbolsOK;
            set { areSymbolsOK = value; OnPropertyChanged("IsOn"); }
        }

        public bool IsOn => IsConnectionOK && AreSymbolsOK;

        #region Monitor
        void StartMonitor() {
            monitorTimer = new Timer(Monitor, null, 1000, Timeout.Infinite);
        }

        void Monitor(object state) {
            monitorTimer.Change(Timeout.Infinite, Timeout.Infinite);

            var healthy = 
                Connection.State == ConnectionState.Connected 
                && 
                Log.ExOnce(() => (bool)Connection.ReadSymbol("GVL.Watchdog_OK", typeof(bool), false), "Unhealthy PLC Connection");

            if (IsConnectionOK && !healthy) {
                Unhook();
                IsConnectionOK = false;
            }
            else
            if (!IsConnectionOK && healthy) {
                Hook();
                IsConnectionOK = true;
            }

            if (IsConnectionOK && !AreSymbolsOK) SetupSymbolsAndNotifications();

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

        void Upload(Axis axis) {
            var values = new object[] {
                axis.MinLoad
                , axis.MaxLoad
                , axis.LoadOffs
                , axis.MinVel
                , axis.MaxVel
            };

            List<ISymbol> valueSyms = new List<ISymbol>();
            valueSyms.AddRange(
                new string[] {
                        nameof(Axis.MinLoad)
                        , nameof(Axis.MaxLoad)
                        , nameof(Axis.LoadOffs)
                        , nameof(Axis.MinVel)
                        , nameof(Axis.MaxVel)
                        }
                .Select(propName => tags[axis, propName].Symbol));

            new SumSymbolWrite(Connection, valueSyms).Write(values);
        }

        static readonly string[] moveProps = new string[] { nameof(Axis.MoveVal), nameof(Axis.Velocity), nameof(Axis.Accel), nameof(Axis.Decel) };
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
                values[0] = move / ax.FeetPerRotation;
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
            var enaProps = new string[] {
                nameof(Axis.MAEnable)
                , nameof(Axis.MREnable)
                , nameof(Axis.JogUpEnable)
                , nameof(Axis.JogDnEnable)
            };

            var aps = new List<(Axis, double)>();
            aps.AddRange(preset.MotorPositions.Select(mp => (VM.Motors[mp.Key] as Axis, mp.Value)));
            aps.AddRange(preset.GroupPositions.Select(gp => (VM.Groups[gp.Key] as Axis, gp.Value)));

            foreach ((Axis ax, double pos) in aps) {
                var values = new object[] {
                    pos / ax.FeetPerRotation
                    , ax.DefVel
                    , ax.DefAcc
                    , ax.DefDec
                };

                valueSyms.Clear();
                valueSyms.AddRange(moveProps.Select(prop => tags[ax, prop].Symbol));
                new SumSymbolWrite(Connection, valueSyms).Write(values);

                enabSyms.AddRange(enaProps.Select(p => tags[ax, p].Symbol));
            }

            var tf = new object[] { true, false, false, false };
            new SumSymbolWrite(Connection, enabSyms).Write(Enumerable.Repeat(tf, enabSyms.Count / 4).SelectMany(tfe => tfe).ToArray());
        }

        public bool SaveGroupMotors(int groupIndex, ushort bitmap) => PlcMethod()?.Invoke(groupIndex, bitmap) == 0;

        public bool GetMotorsGroup(ref ushort[] motorGroups) => PlcMethod(motorGroups)?.Invoke(out motorGroups) == 0;

        delegate int PlcMethodDelegate(params object[] @params);
        delegate int PlcMethodOutDelegate<T>(out T output, params object[] @params);
        PlcMethodDelegate PlcMethod([CallerMemberName]string method = null) {
            int fun(params object[] @params) {
                var success = Connection.TryInvokeRpcMethod("GVL.UI", method, @params, out var res) == AdsErrorCode.NoError;
                return (int)res;
            }
            return IsOn ? fun : new PlcMethodDelegate((_) => -1);
        }

        PlcMethodOutDelegate<T> PlcMethod<T>(T outObj = null, [CallerMemberName]string method = null) where T:class {
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
