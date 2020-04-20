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
    /// Interaction logic for MotorSettingsTopPanel.xaml
    /// </summary>
    public partial class MotorTopPanel : UserControl {
        public MotorTopPanel() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (sender == MotorCancelButton) {
                if (VM.IsMotorSettingsEditing) VM.MotorSettingsEditCancel();
                else
                if (VM.IsGroupSettingsEditing) VM.GroupSettingsEditCancel();
            }
            else
            if (sender == MotorSaveButton) {
                if (VM.IsMotorSettingsEditing) VM.MotorSettingsEditSave();
                else
                if (VM.IsGroupSettingsEditing) VM.GroupSettingsEditSave();
            }
        }
    }
}
