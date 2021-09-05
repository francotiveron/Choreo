using Choreo.UserManagement;
using System.Windows;
using System.Windows.Controls;
using static Choreo.Globals;

namespace Choreo
{
    /// <summary>
    /// Interaction logic for CueingPage.xaml
    /// </summary>
    public partial class CueingPage : UserControl
    {
        public CueingPage()
        {
            InitializeComponent();
        }

        private void NewCueButton_Click(object sender, RoutedEventArgs e) {
            User.RequirePower();
            var cue = new Cue();
            VM.Cues.Add(cue);
            VM.BeginCueEditing(cue.Index);
        }

    }
}
