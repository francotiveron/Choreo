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
    /// Interaction logic for ExceptionPopup.xaml
    /// </summary>
    public partial class ExceptionPopup : Popup {
        public ExceptionPopup() : base() => InitializeComponent();

        public ExceptionPopup(Exception x, string message = null, string caller = null, bool off = false) : this() {
            DataContext = new {
                x
                , message
                , caller
                , off
            };
        }

        private void Close_Button_Click(object sender, RoutedEventArgs e) => Close();
    }
}
