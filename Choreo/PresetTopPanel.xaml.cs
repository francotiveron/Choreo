using System.Windows;
using System.Windows.Controls;
using static Choreo.Globals;

namespace Choreo {
    /// <summary>
    /// Interaction logic for PresetTopPanel.xaml
    /// </summary>
    public partial class PresetTopPanel : UserControl {
        public PresetTopPanel() {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e) {
            var button = (Button)sender;
            switch (button.Name) {
                case "PresetEditSaveButton":
                    VM.PresetEditSave();
                    break;
                case "PresetEditCancelButton":
                    VM.PresetEditCancel();
                    break;
                case "PresetEditClearButton":
                    VM.PresetEditClear();
                    break;
            }
        }
    }
}
