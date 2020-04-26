using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Security.Cryptography;
using System.Windows.Threading;
using TwinCAT;
using TwinCAT.Ads;
using TwinCAT.Ads.Reactive;
using TwinCAT.Ads.SumCommand;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;
using static Choreo.Globals;

namespace Choreo.TwinCAT {
    public interface IPlc {
        void Upload(object obj);
    }

    static public class PlcFactory
    {
        public static IPlc New(string Id)
        {
            if (Id.ToLower() == "demo") return new MockPlc();
            return new AdsPlc().Init();
        }
    }
    
    class AdsPlc: PropertyChangedNotifier, IPlc
    {
        AdsSession adsSession = new AdsSession(AmsNetId.Local, 851);
        IDisposable notificationSubscription;
        IObserver<ValueChangedArgs> symbolNotificationObserver;
        DispatcherTimer monitorTimer;
        TagCollection tags;

        public AdsPlc() { }

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


        bool isOn;
        public bool IsOn { 
            get => isOn; 
            private set { isOn = value; OnPropertyChanged(); } 
        }

        #region Monitor
        void StartMonitor() {
            monitorTimer = new DispatcherTimer();
            monitorTimer.Interval = TimeSpan.FromSeconds(1.0);
            monitorTimer.Tick += (_sender, _ea) => Monitor();
            monitorTimer.Start();
        }

        void Monitor() {
            try {
                bool val = (bool)Connection.ReadSymbol("GVL.Watchdog_OK", typeof(bool), false);
                IsOn = IsOn || Setup();
            }
            catch {
                if (IsOn) {
                    notificationSubscription.Dispose();
                    IsOn = false;
                }
            }
        }

        bool Setup() {
            void Hook() {
                Connection.AdsStateChanged += Conn_AdsStateChanged;
                Connection.AdsNotificationError += Conn_AdsNotificationError;
                Connection.AdsSymbolVersionChanged += Conn_AdsSymbolVersionChanged;
            }

            void Unhook() {
                Connection.AdsStateChanged -= Conn_AdsStateChanged;
                Connection.AdsNotificationError -= Conn_AdsNotificationError;
                Connection.AdsSymbolVersionChanged -= Conn_AdsSymbolVersionChanged;
                IsOn = false;
            }

            try {
                Unhook();
                tags.Symbols = SymbolLoaderFactory.Create(Connection, SymbolLoaderSettings.Default).Symbols;
                Hook();
                return SetupNotifications();
            }
            catch { Unhook(); }
            return false;
        }

        bool SetupNotifications() {
            notificationSubscription?.Dispose();
            SymbolCollection notificationSymbols = new SymbolCollection(tags.Select(tag => tag.Symbol));
            notificationSubscription = notificationSymbols.WhenValueChangedAnnotated().Subscribe(symbolNotificationObserver);

            return true;
        }
        #endregion

        #region Notifications
        private void OnSymbolNotification(ValueChangedArgs vca) => tags[vca.Symbol].Push(vca.Value);
        #endregion

        #region Events
        private void Conn_AdsSymbolVersionChanged(object sender, EventArgs e)
        {
            IsOn = Setup();
        }

        private void Conn_AdsNotificationError(object sender, AdsNotificationErrorEventArgs e)
        {
        }

        private void Conn_AdsStateChanged(object sender, AdsStateChangedEventArgs e)
        {
        }

        private void Session_ConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e) => IsOn &= e.NewState == ConnectionState.Connected;
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
