using System.Windows;
using System.Windows.Controls;
using static Choreo.Globals;

namespace Choreo
{
    /// <summary>
    /// Interaction logic for ShowPage.xaml
    /// </summary>
    public partial class ShowPage : UserControl {
        public ShowPage() {
            InitializeComponent();
            this.ForceRotational.IsChecked = Globals.ForceRotational;
        }

        private void DisableCueButton_Click(object sender, RoutedEventArgs e) => CueList.SelectedCue.Enabled = !CueList.SelectedCue.Enabled;

        private void LoadCueButton_Click(object sender, RoutedEventArgs e) => Plc.Upload(CueList.SelectedCue);

        private void SaveShowButton_Click(object sender, RoutedEventArgs e)
        {
            Storage.PersistToRegistry();
        }
        private void ForceRotational_Click(object sender, RoutedEventArgs e)
        {
            Globals.ForceRotational = !Globals.ForceRotational;
            this.ForceRotational.IsChecked = Globals.ForceRotational;
            var w = Window.GetWindow(this);
            var dc = w.DataContext;
            w.DataContext = null;
            w.DataContext = dc;
        }
    }
}
