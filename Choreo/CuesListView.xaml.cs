using Choreo.UserManagement;
using System.Windows;
using System.Windows.Controls;
using static Choreo.Globals;

namespace Choreo {
    /// <summary>
    /// Interaction logic for CuesListView.xaml
    /// </summary>
    public partial class CuesListView : ModeableListView {
        public CuesListView() {
            InitializeComponent();
        }

        private void CueEditButton_Click(object sender, RoutedEventArgs e) {
            User.RequirePower();
            var button = sender as Button;
            var cue = button.DataContext as Cue;
            VM.BeginCueEditing(cue.Index);
            e.Handled = true;
        }

        private void CueDeleteButton_Click(object sender, RoutedEventArgs e) {
            User.RequirePower();
            var button = sender as Button;
            var cue = button.DataContext as Cue;
            VM.DeleteCue(cue.Index);
            e.Handled = true;
        }
    }
}
