using NLog;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.VisualStyles;
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
#if DEBUG
            Popup<ExceptionPopup>(x, message, caller, off);
#else
            if (!off) PopError(message, "Exception");
#endif
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
            return default; 
        }

        bool? Popup<T>(params object[] @params) where T: Popup {
            return dsp.Invoke(() => {
                var popup = (Popup)Activator.CreateInstance(typeof(T), @params);
                return popup.ShowDialog();
            });
        }

        public void Pop(string message, string caption, AlertPopup.Themes theme, bool withOk, bool withCancel) => Popup<AlertPopup>(message, caption, theme, withOk, withCancel);
        public void PopInfo(string message, string caption) => Pop(message, caption, AlertPopup.Themes.Info, true, false);
        public void PopWarning(string message, string caption) => Pop(message, caption, AlertPopup.Themes.Warning, true, false);
        public void PopError(string message, string caption) => Pop(message, caption, AlertPopup.Themes.Error, true, false);
        public bool OkCancel(string message, string caption) => Popup<AlertPopup>(message, caption, AlertPopup.Themes.Info, true, true) ?? false;

        //public void Alert(string message, string caption) {
        //    _ = MessageBox.Show(Application.Current.MainWindow, message, caption);
        //}

    }
}
