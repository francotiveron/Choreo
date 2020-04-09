using System.Windows;
using System.Windows.Controls;
using static Choreo.Globals;

namespace Choreo {
    /// <summary>
    /// Interaction logic for CueSettingsTopPanel.xaml
    /// </summary>
    public partial class CueTopPanel : UserControl {
        public CueTopPanel() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            var button = (Button)sender;
            switch (button.Name) {
                case "CueEditSaveButton":
                    VM.CueEditSave();
                    break;
                case "CueEditCancelButton":
                    VM.CueEditCancel();
                    break;
            }
        }
    }
}
