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
            var button = sender as Button;
            var cue = button.DataContext as Cue;
            VM.BeginCueEditing(cue.Index);
            e.Handled = true;
        }

        private void CueDeleteButton_Click(object sender, RoutedEventArgs e) {
            var button = sender as Button;
            var cue = button.DataContext as Cue;
            VM.DeleteCue(cue.Index);
            e.Handled = true;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            SelectedCue = e.AddedItems[0] as Cue;
        }

        public Cue SelectedCue {
            get { return (Cue)GetValue(SelectedCueProperty); }
            set { SetValue(SelectedCueProperty, value); }
        }

        public static readonly DependencyProperty SelectedCueProperty =
            DependencyProperty.Register("SelectedCue", typeof(Cue), typeof(CuesListView));

    }
}
