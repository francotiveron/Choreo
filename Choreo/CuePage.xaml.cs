using System.Windows;
using System.Windows.Controls;
using static Choreo.Globals;

namespace Choreo {
    /// <summary>
    /// Interaction logic for CueSettingsPage.xaml
    /// </summary>
    public partial class CuePage : UserControl {
        public CuePage() {
            InitializeComponent();
        }

        private void AddRowButton_Click(object sender, RoutedEventArgs e) {
            var cue = VM.Cues[VM.CueBeingEdited - 1];
            cue.Rows.Add(new CueRow());
        }
    }
}
