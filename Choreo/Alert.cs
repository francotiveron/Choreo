using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Choreo {
    public class Alert {
        public static void Info(string message, string caption) {
            _ = MessageBox.Show(Application.Current.MainWindow, message, caption);
        }
    }
}
