using System;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using static Choreo.Globals;

namespace Choreo
{
    public partial class MotorPanel : UserControl
    {
        DispatcherTimer gestureTimer;
        public MotorPanel()
        {
            InitializeComponent();
            gestureTimer = new DispatcherTimer(TimeSpan.FromSeconds(5), DispatcherPriority.Normal, GestureTimeout, Dispatcher.CurrentDispatcher) { Tag = this };
        }
        public bool IsMotor => DataContext is Motor;
        public bool IsGroup => DataContext is Group;

        public int Index => IsGroup ? ((Group)DataContext).Index : ((Motor)DataContext).Index;

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

        private static void GestureTimeout(object sender, EventArgs e)
        {
            var panel = (MotorPanel)((DispatcherTimer)sender).Tag;
            if (panel.Gesture)
            {
                panel.StopGesture();
                if (panel.IsGroup) VM.BeginGroupEditing(panel.Index);
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
                if (VM.IsGroupEditing) {
                    if (IsMotor) {
                        var m = DataContext as Motor;
                        if (m.Group == 0) m.Group = VM.GroupBeingEdited;
                        else
                        if (m.Group == VM.GroupBeingEdited) m.Group = 0;
                    }
                    return;
                }
                else StartGesture(e.GetPosition(this));
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
            gestureTimer.Interval = TimeSpan.FromSeconds(5);
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
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (VM.GroupBeingEdited > 0) {
                switch (value[0]) {
                    case Motor m: return m.Group == VM.GroupBeingEdited ? Visibility.Hidden : Visibility.Visible;
                    case Group g: return g.Index + 1 == VM.GroupBeingEdited ? Visibility.Hidden : Visibility.Visible;
                }
            }

            return Visibility.Hidden;
        }
        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
    