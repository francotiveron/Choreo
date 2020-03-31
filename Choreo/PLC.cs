using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using TwinCAT.Ads;
using TwinCAT.Ads.Reactive;
using TwinCAT.Ads.TypeSystem;
using TwinCAT.TypeSystem;
using static Choreo.Globals;

namespace Choreo
{
    public interface IPlc {
    }

    static public class PlcFactory
    {
        public static IPlc New(string Id)
        {
            if (Id.ToLower() == "demo") return new MockPlc();
            return new AdsPlc();
        }
    }
    
    class AdsPlc: IPlc
    {
        bool on;
        public AdsPlc()
        {
            Init();
        }
        AdsSession session = new AdsSession(AmsNetId.Local, 851);
        IDisposable subscription;
        ReadOnlySymbolCollection symbols;
        IObserver<SymbolNotification> valueObserver;
        private void Init()
        {
            session.Connect();
            var conn = session.Connection;
            session.ConnectionStateChanged += Session_ConnectionStateChanged;
            conn.AdsStateChanged += Conn_AdsStateChanged;
            conn.AdsNotificationError += Conn_AdsNotificationError;
            conn.AdsSymbolVersionChanged += Conn_AdsSymbolVersionChanged;
            symbols = SymbolLoaderFactory.Create(conn, SymbolLoaderSettings.Default).Symbols;
            //dynamic symbols = session.SymbolServer.Symbols;
            var t = new DispatcherTimer();
            t.Interval = TimeSpan.FromSeconds(1.0);
            t.Tick += T_Tick;
            t.Start();
            int eventCount = 1;

            // Reactive Notification Handler
            valueObserver = Observer.Create<SymbolNotification>(not =>
            {
                VM.Motors[0].IsOK = (bool)not.Value;
                Debug.Print(string.Format("{0} {1:u} {2} = '{3}' ({4})", eventCount++, not.TimeStamp, not.Symbol.InstancePath, not.Value, not.Symbol.DataType));
            }
            );

            // Collect the symbols that are registered as Notification sources for their changed values.

            SymbolCollection notificationSymbols = new SymbolCollection();
            ISymbol sym = symbols["GVL.ES1_LED"];
            notificationSymbols.Add(sym);

            // Create a subscription for the first 200 Notifications on Symbol Value changes.
            subscription = conn.WhenNotification(notificationSymbols, NotificationSettings.Default).Subscribe(valueObserver);
            on = true;
        }

        private void T_Tick(object sender, EventArgs e)
        {
            try
            {
                bool val = (bool)session.Connection.ReadSymbol("GVL.ES1_LED", typeof(bool), false);
                if (!on)
                {
                    SymbolCollection notificationSymbols = new SymbolCollection();
                    ISymbol sym = symbols["GVL.ES1_LED"];
                    notificationSymbols.Add(sym);
                    subscription = session.Connection.WhenNotification(notificationSymbols, NotificationSettings.Default).Subscribe(valueObserver);
                    on = true;
                }
            }
            catch { 
                if (on)
                {
                    subscription.Dispose();
                    on = false;
                }
            }
        }

        private void Conn_AdsSymbolVersionChanged(object sender, EventArgs e)
        {
        }

        private void Conn_AdsNotificationError(object sender, AdsNotificationErrorEventArgs e)
        {
        }

        private void Conn_AdsStateChanged(object sender, AdsStateChangedEventArgs e)
        {
        }

        private void Session_ConnectionStateChanged(object sender, TwinCAT.ConnectionStateChangedEventArgs e)
        {
        }
    }

    class MockPlc : IPlc
    {

    }
}
