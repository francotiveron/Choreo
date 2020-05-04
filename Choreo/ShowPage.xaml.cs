using Choreo.TwinCAT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
