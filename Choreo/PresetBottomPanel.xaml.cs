using Choreo.UserManagement;
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using static Choreo.Globals;

namespace Choreo {
    /// <summary>
    /// Interaction logic for PresetBottomPanel.xaml
    /// </summary>
    public partial class PresetBottomPanel : UserControl {
        public PresetBottomPanel() {
            InitializeComponent();
        }

        Preset preset;
        private void PushTimeout() {
            var n = preset?.Number ?? 0;
            preset = null;
            if (VM.IsEditing || n == 0) return;
            User.RequirePower();
            VM.BeginPresetEditing(n - 1);
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e) {
            if (VM.IsEditing) return;
            if (e.ChangedButton == MouseButton.Left) {
                preset = (sender as Button).DataContext as Preset;
                PushTimer.Start(() => PushTimeout());
            }
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e) {
            if (VM.IsEditing) return;
            if (PushTimer.Stop() && preset != null && !preset.IsEmpty) {
                User.RequireLimited();
                Plc.Upload(preset);
            }
        }
    }
    public class PresetBottomPanelButtonColorConverter : IMultiValueConverter {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture) {
            try {
                var preset = (Preset)value[0];
                var edited = (int)value[1];
                var loaded = (int)value[2];
                var moveActive = (bool)value[3];

                if (preset.Number == edited) return Brushes.Aquamarine;
                else if (preset.IsEmpty) return Brushes.Gray;
                else return preset.Number == loaded ? (moveActive ? Brushes.Lime : Brushes.Cyan) : Brushes.DarkBlue;
            }
            catch { }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    public class PresetBottomPanelButtonBorderColorConverter : IMultiValueConverter {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture) {
            try {
                var preset = (Preset)value[0];
                var moveActive = (bool)value[1];

                return preset.Realised && !moveActive ? Brushes.Gold : null;
            }
            catch { }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
