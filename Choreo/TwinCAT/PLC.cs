using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Threading;
using TwinCAT;
using TwinCAT.Ads;
using TwinCAT.Ads.Reactive;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;
using static Choreo.Globals;

namespace Choreo.TwinCAT {
    public interface IPlc {
        void Init();
        void Write(string path, object value);
        object Read(string path);
    }

    static public class PlcFactory
    {
        public static IPlc New(string Id)
        {
            if (Id.ToLower() == "demo") return new MockPlc();
            return new AdsPlc();
        }
    }
    
    class AdsPlc: PropertyChangedNotifier, IPlc
    {
        AdsSession adsSession = new AdsSession(AmsNetId.Local, 851);
        IDisposable notificationSubscription;
        ReadOnlySymbolCollection adsSymbols;
        //IObserver<SymbolNotification> symbolNotificationObserver;
        IObserver<ValueChangedArgs> symbolNotificationObserver;
        DispatcherTimer monitorTimer;
        TagCollection tags;

        public AdsPlc() { }

        public void Init()
        {
            adsSession.Connect();
            adsSession.ConnectionStateChanged += Session_ConnectionStateChanged;

            var adsConnection = adsSession.Connection;
            adsConnection.AdsStateChanged += Conn_AdsStateChanged;
            adsConnection.AdsNotificationError += Conn_AdsNotificationError;
            adsConnection.AdsSymbolVersionChanged += Conn_AdsSymbolVersionChanged;

            adsSymbols = SymbolLoaderFactory.Create(adsConnection, SymbolLoaderSettings.Default).Symbols;

            tags = TagCollection.Instance;

            symbolNotificationObserver = Observer.Create<ValueChangedArgs>(OnSymbolNotification);

            StartMonitor();
            //    not =>
            //{
            //    VM.Motors[0].IsOK = (bool)not.Value;
            //    //Debug.Print(string.Format("{0} {1:u} {2} = '{3}' ({4})", eventCount++, not.TimeStamp, not.Symbol.InstancePath, not.Value, not.Symbol.DataType));
            //}
            //);

            // Collect the symbols that are registered as Notification sources for their changed values.
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
                bool val = (bool)adsSession.Connection.ReadSymbol("GVL.ES1_LED", typeof(bool), false);
                IsOn = IsOn || SetupNotifications();
            }
            catch {
                if (IsOn) {
                    notificationSubscription.Dispose();
                    IsOn = false;
                }
            }
        }

        bool SetupNotifications() {
            SymbolCollection notificationSymbols = new SymbolCollection(tags.Select(tag => adsSymbols[tag.Path]));
            notificationSubscription = notificationSymbols.WhenValueChangedAnnotated().Subscribe(symbolNotificationObserver);

            return true;
        }
        #endregion

        #region Notifications
        private void OnSymbolNotification(ValueChangedArgs vca) {
            var path = vca.Symbol.InstancePath;
            tags[path].Push(vca.Value);
        }
        #endregion

        #region Events
        private void Conn_AdsSymbolVersionChanged(object sender, EventArgs e)
        {
        }

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
        public void Write(string path, object value) {
            throw new NotImplementedException();
        }

        public object Read(string path) {
            throw new NotImplementedException();
        }

        #endregion
    }

    class MockPlc : IPlc {
        public void Init() {
            throw new NotImplementedException();
        }

        public object Read(string path) {
            throw new NotImplementedException();
        }

        public void Write(string path, object value) {
            throw new NotImplementedException();
        }
    }
}
