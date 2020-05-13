using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Choreo.Logging {
    /// <summary>
    /// Interaction logic for AlertPopup.xaml
    /// </summary>
    public partial class AlertPopup : Popup {
        public AlertPopup() : base() => InitializeComponent();
        public AlertPopup(string message, string caption = null, bool ok = false, bool cancel = false) : this() {
            OkButton.Visibility = ok ? Visibility.Visible : Visibility.Collapsed;
            CancelButton.Visibility = cancel ? Visibility.Visible : Visibility.Collapsed;
            DataContext = new { caption, message };
        }

        private void OkButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
            Close();
        }
    }
}
