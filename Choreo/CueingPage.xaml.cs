using System.Windows;
using System.Windows.Controls;
using static Choreo.Globals;

namespace Choreo {
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
            var newCueIndex = VM.Cues.Count;
            VM.Cues.Add(new Cue(newCueIndex));
            VM.BeginCueEditing(newCueIndex);
        }
    }
}
