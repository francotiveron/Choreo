using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Choreo
{
    /// <summary>
    /// Interaction logic for MotorStatusIndicator.xaml
    /// </summary>
    public partial class MotorStatusIndicator : UserControl
    {
        public MotorStatusIndicator()
        {
            InitializeComponent();
        }

        //private void UserControl_Loaded(object sender, RoutedEventArgs e) {
        //    if (DataItemUI.ValueFontSize != null) Status.SetValue(FontSizeProperty, DataItemUI.ValueFontSize);
        //}
    }

    public class MotorStatusConverter : IMultiValueConverter {
        static string GetText(Axis axis) {
            if (axis.MAEnable || axis.MREnable) {
                return $"{(axis.MAEnable ? "A" : "R")}: {FeetInchesConvert.ToString(axis.MoveVal)}";
            }
            else
            if (axis.JogUpEnable) return "Jog Up";
            else
            if (axis.JogDnEnable) return "Jog Dn";
            else return "OK";
        }

        static readonly SolidColorBrush okBrush = new SolidColorBrush(Colors.Lime);
        static Brush GetColor(Axis axis) {
            return okBrush;
        }

        static Dictionary<Type, Func<Axis, object>> funcs = new Dictionary<Type, Func<Axis, object>>() { { typeof(object), GetText }, { typeof(Brush), GetColor } };
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            if (funcs.TryGetValue(targetType, out var func)) return func(values[0] as Axis);
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
