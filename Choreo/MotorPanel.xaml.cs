using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
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
using System.Windows.Threading;
using TwinCAT.PlcOpen;
using static Choreo.Globals;

namespace Choreo
{
    public partial class MotorPanel : UserControl
    {
        public MotorPanel()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var uc = (MotorPanel)sender;
            var row = (int)uc.GetValue(Grid.RowProperty);
            var col = (int)uc.GetValue(Grid.ColumnProperty);
            var index = row * 8 + col;
            gestureTimer.Tag = this;
            if ((string)uc.Parent.GetValue(Grid.NameProperty) == "AxisMonitorGrid")
                DataContext = VM.Motors[index];
            else
                DataContext = VM.Groups[index];
        }

        public bool IsMotor => DataContext is Motor;
        public bool IsGroup => DataContext is Group;

        #region Gesture
        bool gesture;
        bool Gesture {
            get => gesture;
            set {
                gesture = value;
                Mouse.OverrideCursor = gesture ? Cursors.ScrollWE : null;
            }
        }
        double gestureStart;
        DispatcherTimer gestureTimer = new DispatcherTimer(TimeSpan.FromSeconds(5), DispatcherPriority.Normal, GestureTimeout, Dispatcher.CurrentDispatcher);

        private static void GestureTimeout(object sender, EventArgs e)
        {
            var panel = (MotorPanel)((DispatcherTimer)sender).Tag;
            if (panel.Gesture)
            {
                panel.StopGesture();
                Debug.Print("Settings");
            }
        }

        private void Border_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {

        }

        private void Border_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {

        }

        private void Border_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {

        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                StartGesture(e.GetPosition(this));
            }
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (Gesture)
            {
                var x = e.GetPosition(this).X;
                if ((x - gestureStart) < -10.0) GestureLeft();
                if ((x - gestureStart) > 10.0) GestureRight();
            }
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            StopGesture();
        }

        private void StartGesture(Point p)
        {
            gestureStart = p.X;
            gestureTimer.Start();
            Gesture = true;
        }

        private void StopGesture()
        {
            gestureTimer.Stop();
            Gesture = false;
        }

        private void GestureLeft()
        {
            Gesture = false;
        }

        private void GestureRight()
        {
            Gesture = false;
        }

        #endregion
    }

    public class MotorPanelDarkeningConverter: IMultiValueConverter {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal Amount = 0;
            decimal Discount = 0;
            string TotalAmount = string.Empty;
            Amount = (values[0] != null && values[0] != DependencyProperty.UnsetValue) ? System.Convert.ToDecimal(values[0]) : 0;
            Discount = (values[0] != null && values[1] != DependencyProperty.UnsetValue) ? System.Convert.ToDecimal(values[1]) : 0;
            TotalAmount = System.Convert.ToString(Amount - Discount);
            return TotalAmount;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
    