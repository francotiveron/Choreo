using Choreo.Logging;
using System.Linq;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == MotorCancelButton)
            {
                if (VM.IsMotorSettingsEditing) VM.MotorSettingsEditCancel();
                else
                if (VM.IsGroupSettingsEditing) VM.GroupSettingsEditCancel();
            }
            else
            if (sender == MotorSaveButton)
            {
                var window = Application.Current.MainWindow as MotorWindow;
                var page = window?.Page;
                if (DataItemsValid(page.EditableElementsGrid.Children))
                {
                    Axis editAxis = null;
                    if (VM.IsMotorSettingsEditing) editAxis = VM.Motors[VM.MotorSettingsBeingEdited - 1];
                    else
                    if (VM.IsGroupSettingsEditing) editAxis = VM.Groups[VM.GroupSettingsBeingEdited - 1];

                    var popup = new AxesSetupPopup(editAxis);
                    if (popup.ShowDialog() ?? false)
                    {
                        var indexes = popup.Selected().ToArray();
                        foreach (int i in indexes) CopySettings(i);
                        VM.AxesSettingsEditSave(indexes);

                        void CopySettings(int target)
                        {
                            Axis targetAxis = null;

                            if (target < 16) targetAxis = VM.Motors[target];
                            else targetAxis = VM.Groups[target - 16];

                            targetAxis.MinLoad = editAxis.MinLoad;
                            targetAxis.MaxLoad = editAxis.MaxLoad;
                            targetAxis.LoadOffs = editAxis.LoadOffs;
                            targetAxis.MinVel = editAxis.MinVel;
                            targetAxis.MaxVel = editAxis.MaxVel;
                            targetAxis.DefVel = editAxis.DefVel;
                            targetAxis.MaxAcc = editAxis.MaxAcc;
                            targetAxis.DefAcc = editAxis.DefAcc;
                            targetAxis.MaxDec = editAxis.MaxDec;
                            targetAxis.DefDec = editAxis.DefDec;
                            targetAxis.SoftDnRotations = editAxis.SoftDnRotations;
                            targetAxis.SoftUpRotations = editAxis.SoftUpRotations;
                            targetAxis.LoadCellActive = editAxis.LoadCellActive;
                        }
                    }
                }
                else Log.PopInfo("Please insert valid data", "Invalid Data Entry");
            }
        }
    }
}
