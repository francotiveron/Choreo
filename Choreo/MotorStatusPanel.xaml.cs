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
        static readonly Brush limeBrush = new SolidColorBrush(Colors.Lime);
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            var axis = values[0] as Axis;
            Brush
                @default = values[1] as Brush,
                jogUp = values[2] as Brush,
                jogDn = values[3] as Brush,
                move = values[4] as Brush,
                brush = null;

            if ((string)parameter == "Rectangle") {
                if (axis.MAEnable || axis.MREnable || axis.JogUpEnable || axis.JogDnEnable)
                    brush = limeBrush;
            }
            else if (axis.Active) brush = move;
            else if (axis.JogUpEnable) brush = jogUp;
            else if (axis.JogDnEnable) brush = jogDn;
            else brush = @default;

            return brush;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
