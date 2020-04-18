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

            if (VM.IsGroupEditing && DataContext is Motor m) {
                if (m.Group == 0) {
                    if (m.IsPreset) {
                        Alert.Info("Motors already in Presets cannot be grouped", "Motor in Preset");
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
            VM.BeginMotionEditing(true, DataContext);
            Gesture = false;
        }

        private void GestureRight()
        {
            VM.BeginMotionEditing(false, DataContext);
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
    