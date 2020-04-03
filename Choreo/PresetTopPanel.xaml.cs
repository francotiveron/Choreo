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

namespace Choreo {
    /// <summary>
    /// Interaction logic for PresetTopPanel.xaml
    /// </summary>
    public partial class PresetTopPanel : UserControl {
        public PresetTopPanel() {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e) {
            var button = (Button)sender;
            switch (button.Name) {
                case "PresetEditSaveButton":
                    VM.PresetEditSave();
                    break;
                case "PresetEditCancelButton":
                    VM.PresetEditCancel();
                    break;
                case "PresetEditClearButton":
                    VM.PresetEditClear();
                    break;
            }
        }
    }
}
