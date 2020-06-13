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
                    if (VM.Cues[VM.CueBeingEdited - 1].IsValid) VM.CueEditSave();
                    else Log.PopInfo("Please insert valid data", "Invalid Data Entry");
                    break;
                case "CueEditCancelButton":
                    VM.CueEditCancel();
                    break;
                case "CueAddRowButton":
                    var cue = VM.Cues[VM.CueBeingEdited - 1];
                    cue.Rows.Add(new CueRow(cue));
                    break;
            }
        }
    }
}
