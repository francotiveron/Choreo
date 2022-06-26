using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

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

    public class MotorStatusColorConverter : IMultiValueConverter {

        static readonly SolidColorBrush okBrush = new SolidColorBrush(Colors.Lime);
        static readonly SolidColorBrush warnBrush = new SolidColorBrush(Colors.Yellow);
        static readonly SolidColorBrush alertBrush = new SolidColorBrush(Colors.Orange);
        static readonly SolidColorBrush errBrush = new SolidColorBrush(Colors.Red);
        static readonly Brush[] brushes = new Brush[] { okBrush, warnBrush, alertBrush, errBrush };
        static Brush GetColor(Axis axis) => brushes[(int)axis.AxisStatus];

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => GetColor(values[0] as Axis);

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    public class MotorStatusTextConverter : IMultiValueConverter
    {
        static string GetText(Axis axis) => axis.AxisStatusDescription;
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => GetText(values[0] as Axis);

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
