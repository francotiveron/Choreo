using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Choreo.Logging
{
    /// <summary>
    /// Interaction logic for AlertPopup.xaml
    /// </summary>
    public partial class AlertPopup : Popup {
        public enum Themes { Info, Warning, Error }
        public AlertPopup() : base() => InitializeComponent();
        public AlertPopup(string message, string caption = null, Themes theme = Themes.Info, bool ok = false, bool cancel = false) : this() {
            OkButton.Visibility = ok ? Visibility.Visible : Visibility.Collapsed;
            CancelButton.Visibility = cancel ? Visibility.Visible : Visibility.Collapsed;
            DataContext = new { caption, message, theme };
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

    public class AlertPopupConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var theme = (AlertPopup.Themes)value;
            if (targetType == typeof(Brush)) {
                var color = Colors.Black;
                switch (theme) {
                    case AlertPopup.Themes.Info:
                        color = Colors.Blue;
                        break;
                    case AlertPopup.Themes.Warning:
                        color = (Color)ColorConverter.ConvertFromString("#FFF1AA10");
                        break;
                    case AlertPopup.Themes.Error:
                        color = Colors.Red;
                        break;
                }
                return new SolidColorBrush(color);
            }
            else {
                var ret = "";

                switch (theme) {
                    case AlertPopup.Themes.Info:
                        ret = @"/Choreo;component/Pictures/InfoSign.png";
                        break;
                    case AlertPopup.Themes.Warning:
                        ret = @"/Choreo;component/Pictures/WarningSign.png";
                        break;
                    case AlertPopup.Themes.Error:
                        ret = @"/Choreo;component/Pictures/ErrorSign.png";
                        break;
                }
                return ret;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
