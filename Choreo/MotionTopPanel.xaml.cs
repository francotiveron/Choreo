using System.Windows;
using System.Windows.Controls;
using static Choreo.Globals;

namespace Choreo {
    /// <summary>
    /// Interaction logic for MotionTopPanel.xaml
    /// </summary>
    public partial class MotionTopPanel : UserControl {
        public MotionTopPanel() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) => VM.MotionEditing = false;
    }
}
