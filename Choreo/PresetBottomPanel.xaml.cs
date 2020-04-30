using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
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

        private void PushTimeout(int presetIndex) {
            if (VM.IsEditing) return;
            VM.BeginPresetEditing(presetIndex);
        }

        private void Button_MouseDown(object sender, MouseButtonEventArgs e) {
            var button = (Button)sender;
            var presetIndex = (int)button.GetValue(Grid.ColumnProperty);
            if (e.LeftButton == MouseButtonState.Pressed) PushTimer.Start(() => PushTimeout(presetIndex));
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e) {
            PushTimer.Stop();
        }
    }
    public class PresetBottomPanelButtonColorConverter : IMultiValueConverter {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture) {
            try {
                var preset = (Preset)value[0];
                var edited = (int)value[1];
                if (preset.Number == edited) return Colors.Aquamarine;
                else
                if (!preset.IsEmpty) return Colors.Blue;
                else return Colors.DarkBlue;
            }
            catch { }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
