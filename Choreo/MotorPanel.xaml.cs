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
        public MotorPanel()
        {
            InitializeComponent();
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
            if (e.LeftButton != MouseButtonState.Pressed) return;

            if (VM.IsEditing) {
                if (IsMotor) {
                    var m = DataContext as Motor;
                    if (VM.IsGroupEditing) {
                        if (m.Group == 0) m.Group = VM.GroupBeingEdited;
                        else
                        if (m.Group == VM.GroupBeingEdited) m.Group = 0;
                        return;
                    }
                    else
                    if (VM.IsPresetEditing) VM.Presets[VM.PresetBeingEdited - 1].Toggle(m);
                }
            }
            else StartGesture(e.GetPosition(this));
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

        private void PushTimeout() {
            if (Gesture) {
                StopGesture();
                if (IsGroup && !VM.IsEditing) VM.BeginGroupEditing(Index);
            }
        }

        private void StartGesture(Point p)
        {
            if (PushTimer.Start(PushTimeout)) {
                Gesture = true;
                gestureStart = p.X;
            }
        }

        private void StopGesture()
        {
            PushTimer.Stop();
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
            if (VM.IsGroupEditing) {
                switch (value[0]) {
                    case Motor m: return m.Group == VM.GroupBeingEdited ? Visibility.Hidden : Visibility.Visible;
                    case Group g: return g.Index + 1 == VM.GroupBeingEdited ? Visibility.Hidden : Visibility.Visible;
                }
            }
            else
            if (VM.IsPresetEditing) {
                switch (value[0]) {
                    case Motor m: return VM.Presets[VM.PresetBeingEdited - 1].ContainsMotor(m.Index) ? Visibility.Hidden : Visibility.Visible;
                    default: return Visibility.Visible;
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
    