using System;
using System.Windows;

namespace Choreo.Logging
{
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
