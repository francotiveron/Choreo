using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Choreo {
    public static class PushTimer {
        static DispatcherTimer timer;
        static readonly TimeSpan interval = TimeSpan.FromSeconds(5);

        public static bool Start(Action callback) {
            if (timer == null) {
                timer = new DispatcherTimer() {
                    Interval = interval,
                    Tag = callback
                };
                timer.Tick += Timer_Tick;
                timer.Start();
                return true;
            }
            return false;
        }

        public static bool Stop() {
            if (timer != null) {
                timer.Stop();
                timer = null;
                return true;
            }
            return false;
        }

        private static void Timer_Tick(object sender, EventArgs e) {
            var callback = (Action)timer.Tag;
            Stop();
            callback();
        }
    }
}
