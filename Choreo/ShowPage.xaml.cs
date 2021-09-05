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
        }

        private void DisableCueButton_Click(object sender, RoutedEventArgs e) => CueList.SelectedCue.Enabled = !CueList.SelectedCue.Enabled;

        private void LoadCueButton_Click(object sender, RoutedEventArgs e) => Plc.Upload(CueList.SelectedCue);
    }
}
