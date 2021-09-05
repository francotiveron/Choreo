using System.Windows;
using System.Windows.Controls;
using static Choreo.Globals;

namespace Choreo
{
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
                var window = Application.Current.MainWindow as MotorWindow;
                var page = window?.Page;
                if (DataItemsValid(page.EditableElementsGrid.Children)) {
                    if (VM.IsMotorSettingsEditing) VM.MotorSettingsEditSave();
                    else
                    if (VM.IsGroupSettingsEditing) VM.GroupSettingsEditSave();
                }
                else Log.PopInfo("Please insert valid data", "Invalid Data Entry");
            }
        }
    }
}
