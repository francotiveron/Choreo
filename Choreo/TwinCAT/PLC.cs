using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
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
        void Upload(object obj);
    }

    static public class PlcFactory
    {
        public static IPlc New(string Id = null)
        {
            if (Id?.ToLower() == "demo") return new MockPlc();
            return new AdsPlc(Cfg.PlcAmsNetId, Cfg.PlcAmsPort).Init();
        }
    }
    
    class AdsPlc: PropertyChangedNotifier, IPlc
    {
        AdsSession adsSession;
        IDisposable notificationSubscription;
        IObserver<ValueChangedArgs> symbolNotificationObserver;
        Timer monitorTimer;
        TagCollection tags;

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
            
            bool IsHealthy() {
                if (Connection.State == ConnectionState.Connected) {
                    try {
                        bool val = (bool)Connection.ReadSymbol("GVL.Watchdog_OK", typeof(bool), false);
                        return true;
                    }
                    catch { return false; }
                }
                return false;
            }

            var healthy = IsHealthy();

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
            try {
                notificationSubscription?.Dispose();
                tags.Symbols = SymbolLoaderFactory.Create(Connection, SymbolLoaderSettings.Default).Symbols;
                SymbolCollection notificationSymbols = new SymbolCollection(tags.Select(tag => tag.Symbol));
                notificationSubscription = notificationSymbols.WhenValueChangedAnnotated().Subscribe(symbolNotificationObserver);
                AreSymbolsOK = true;
            }
            catch { }
        }
        private void OnSymbolNotification(ValueChangedArgs vca) => tags[vca.Symbol].Push(vca.Value);
        #endregion

        #region Events
        private void Conn_AdsSymbolVersionChanged(object sender, EventArgs e) => AreSymbolsOK = false;

        private void Conn_AdsNotificationError(object sender, AdsNotificationErrorEventArgs e)
        {
        }

        private void Conn_AdsStateChanged(object sender, AdsStateChangedEventArgs e)
        {
        }

        private void Session_ConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e) 
        { 
        }
        #endregion

        #region IPlc Implementation
        public void Upload(object obj) {
            switch(obj) {
                case Motion motion: UploadCore(motion); break;
            }
        }

        void UploadCore(Motion motion) {
            var values = new object[] {
                motion.Relative ? motion.RelativeSetpoint : motion.AbsoluteSetpoint
                , motion.Velocity
                , motion.Acceleration
                , motion.Deceleration
            };

            List<ISymbol> enabSyms = new List<ISymbol>();
            var enableProp = motion.Relative ? nameof(Motor.MREnable) : nameof(Motor.MAEnable);

            for(int i = 0; i < motion.Motors.Length; i++) {
                if (!motion.Motors[i]) continue;

                enabSyms.Add(tags[VM.Motors[i], enableProp].Symbol);

                List<ISymbol> valueSyms = new List<ISymbol>();
                valueSyms.AddRange(
                    new string[] {
                        nameof(Motor.MoveVal)
                        , nameof(Motor.Velocity)
                        , nameof(Motor.Accel)
                        , nameof(Motor.Decel) }
                    .Select(propName => tags[VM.Motors[i], propName].Symbol));

                new SumSymbolWrite(Connection, valueSyms).Write(values);
            }

            new SumSymbolWrite(Connection, enabSyms).Write(Enumerable.Repeat((object)true, enabSyms.Count).ToArray());
        }

        #endregion
    }

    class MockPlc : IPlc {
        #region IPlc Implementation
        public void Upload(object obj) {
            throw new NotImplementedException();
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
