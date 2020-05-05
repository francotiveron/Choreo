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
        public bool AxisEnabled => DataContext is Axis axis && axis.UserEnable;

        public int Index => IsGroup ? ((Group)DataContext).Index : ((Motor)DataContext).Index;

        #region Gesture
        bool gesture;
        bool Gesture {
            get => gesture;
            set {
                gesture = value;
                Mouse.OverrideCursor = gesture ? Cursors.ScrollAll : null;
            }
        }
        double gestureStartX, gestureStartY;

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

            if (VM.IsGroupEditing && DataContext is Motor m) {
                if (m.Group == 0) {
                    if (m.IsPreset) {
                        Log.Alert("Motors already in Presets cannot be grouped", "Motor in Preset");
                        return;
                    }
                    m.Group = VM.GroupBeingEdited;
                }
                else
                if (m.Group == VM.GroupBeingEdited) m.Group = 0;
                return;
            }
            else
            if (VM.IsPresetEditing) { 
                if (!(DataContext is Motor mm) || !mm.IsGrouped) VM.Presets[VM.PresetBeingEdited - 1].Toggle(DataContext); 
            }
            else StartGesture(e.GetPosition(this));
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (Gesture && AxisEnabled)
            {
                var x = e.GetPosition(this).X;
                var y = e.GetPosition(this).Y;
                if ((x - gestureStartX) < -10.0) GestureLeft();
                else
                if ((x - gestureStartX) > 10.0) GestureRight();
                else
                if ((y - gestureStartY) < -10.0) GestureUp();
                else
                if ((y - gestureStartY) > 10.0) GestureDn();
                e.Handled = true;
            }
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            StopGesture();
        }

        private void PushTimeout() {
            if (Gesture) {
                StopGesture();
                if (!VM.IsEditing) {
                    if (IsGroup) VM.BeginGroupEditing(Index);
                    else
                    if (IsMotor) VM.BeginMotorSettingsEditing(Index);
                }
            }
        }

        private void StartGesture(Point p)
        {
            if (PushTimer.Start(PushTimeout)) {
                Gesture = true;
                gestureStartX = p.X;
                gestureStartY = p.Y;
            }
        }

        private void StopGesture()
        {
            PushTimer.Stop();
            Gesture = false;
        }

        private void GestureLeft()
        {
            VM.BeginMotionEditing(true, (Axis)DataContext);
            Gesture = false;
        }

        private void GestureRight()
        {
            VM.BeginMotionEditing(false, (Axis)DataContext);
            Gesture = false;
        }

        private void GestureUp() {
            var axis = (Axis)DataContext;
            if (!axis.JogUpEnable) {
                if (axis.JogDnEnable) Plc.Jog(axis, 0);
                else Plc.Jog(axis, 1);
            }
            Gesture = false;
        }

        private void GestureDn() {
            var axis = (Axis)DataContext;
            if (!axis.JogDnEnable) {
                if (axis.JogUpEnable) Plc.Jog(axis, 0);
                else Plc.Jog(axis, -1);
            }
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
                var preset = VM.Presets[VM.PresetBeingEdited - 1];
                switch (value[0]) {
                    case Motor m: return preset.ContainsMotor(m.Index) ? Visibility.Hidden : Visibility.Visible;
                    case Group g: return preset.ContainsGroup(g.Index) ? Visibility.Hidden : Visibility.Visible;
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
    