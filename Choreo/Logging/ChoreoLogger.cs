using NLog;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;

namespace Choreo.Logging {
    public class ChoreoLogger : Logger {
        static readonly Dispatcher dsp = Application.Current.Dispatcher;

        public static ChoreoLogger GetLogging() {
            LogManager.AutoShutdown = true;
            return (ChoreoLogger)LogManager.GetLogger("Logging", typeof(ChoreoLogger));
        }
        
        public void Exception(Exception x, string message = null, [CallerMemberName] string caller = null, bool off = false) {
            if (!off) Error(x, message);
            Popup(typeof(ExceptionPopup), x, message, caller, off);
        }

        Dictionary<string, Exception> exOnceDict = new Dictionary<string, Exception>();
        public void ExOnce(Action code, string message = null, [CallerMemberName] string caller = null) => ExOnce<int>(() => { code(); return 0; }, message, caller);
        public T ExOnce<T>(Func<T> code, string message = null, [CallerMemberName] string caller = null) {
            if (caller == null) return default(T);
            try {
                var ret = code();
                if (exOnceDict.TryGetValue(caller, out var x)) {
                    Exception(x, message, caller, off: true);
                    exOnceDict.Remove(caller);
                }
                return ret;
            }
            catch (Exception x) {
                if (!exOnceDict.ContainsKey(caller)) {
                    exOnceDict[caller] = x;
                    Exception(x, message, caller);
                }
            }
            return default(T); 
        }


        void Popup(Type popupType, params object[] @params) {
            dsp.Invoke(() => {
                var popup = (Popup)Activator.CreateInstance(popupType, @params);
                popup.ShowDialog();
            });
        }

        public void Alert(string message, string caption) {
            _ = MessageBox.Show(Application.Current.MainWindow, message, caption);
        }

    }
}
