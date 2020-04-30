using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Choreo {
    /// <summary>
    /// Interaction logic for MotorStatusPanel.xaml
    /// </summary>
    public partial class MotorStatusPanel : UserControl {
        public MotorStatusPanel() {
            InitializeComponent();
        }
    }

    public class UpDnArrowVisibilityConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            var axis = values[0] as Axis;
            return
                (string)parameter == "Up" && (axis.MAEnable && axis.MoveVal > axis.Position || axis.MREnable && axis.MoveVal > 0 || axis.JogUpEnable)
                ||
                (string)parameter == "Dn" && (axis.MAEnable && axis.MoveVal < axis.Position || axis.MREnable && axis.MoveVal < 0 || axis.JogDnEnable)
                ?
                Visibility.Visible
                :
                Visibility.Hidden;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    public class AxisStatusBrushConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            var axis = values[0] as Axis;
            Brush brush = null;

            if ((string)parameter == "Rectangle") {
                if (axis.MAEnable || axis.MREnable || axis.JogUpEnable || axis.JogDnEnable)
                    brush = new SolidColorBrush(Colors.Lime);
            }
            else if (axis.JogUpEnable) brush = (Brush)values[1];
            else if (axis.JogDnEnable) brush = (Brush)values[2];
            else brush = (Brush)values[3];
            return brush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
