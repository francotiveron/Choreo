using System.Windows.Controls;

namespace Choreo.Input
{
    /// <summary>
    /// Interaction logic for MotorAndGroupSelectorPanel.xaml
    /// </summary>
    public partial class MaGSelectorPanel : UserControl {
        public MaGSelectorPanel() {
            InitializeComponent();
            //MotorCheckBoxes = MotorsCheckGrid.Children.OfType<CheckBox>().ToArray();
            //GroupCheckBoxes = GroupsCheckGrid.Children.OfType<CheckBox>().ToArray();
        }

        //CheckBox[] MotorCheckBoxes { get; set; }
        //CheckBox[] GroupCheckBoxes { get; set; }

        //public void GetMotorsFrom(bool[] motors) {
        //    for (int i = 0; i < Math.Min(MotorCheckBoxes.Length, motors.Length); i++) MotorCheckBoxes[i].IsChecked = motors[i];
        //}
        //public void PutMotorsInto(bool[] motors) {
        //    for (int i = 0; i < Math.Min(MotorCheckBoxes.Length, motors.Length); i++) motors[i] = MotorCheckBoxes[i].IsChecked == true;
        //}
        //public void GetGroupsFrom(bool[] groups) {
        //    for (int i = 0; i < Math.Min(GroupCheckBoxes.Length, groups.Length); i++) GroupCheckBoxes[i].IsChecked = groups[i];
        //}
        //public void PutGroupsInto(bool[] groups) {
        //    for (int i = 0; i < Math.Min(GroupCheckBoxes.Length, groups.Length); i++) groups[i] = GroupCheckBoxes[i].IsChecked == true;
        //}
    }

    public class MaGSelectorDataContextType {
        public bool[] AvailableMotors { get; set; }
        public bool[] AvailableGroups { get; set; }
        public bool[] SelectedMotors { get; set; }
        public bool[] SelectedGroups { get; set; }
    }
}
