using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Choreo.Input
{
    /// <summary>
    /// Interaction logic for MaGSelectionPopup.xaml
    /// </summary>
    public partial class MaGSelectionPopup : UserControl {
        public MaGSelectionPopup() {
            InitializeComponent();
            Visibility = DataItem == null ? Visibility.Hidden : Visibility.Visible;
        }

        MotAndGroUI dataItem;
        public MotAndGroUI DataItem {
            get => dataItem;
            set {
                dataItem = value;
                if (dataItem == null) Visibility = Visibility.Hidden;
                else {
                    var cueRow = dataItem.DataContext as CueRow;
                    var sdc = new MaGSelectorDataContextType {
                        AvailableMotors = AvailableMotors(cueRow)
                        , AvailableGroups = AvailableGroups(cueRow)
                        , SelectedMotors = (bool[])cueRow.Motors.Clone()
                        , SelectedGroups = (bool[])cueRow.Groups.Clone()
                    };
                    SelectorPanel.DataContext = sdc;
                    //SelectorPanel.GetMotorsFrom(cueRow.Motors);
                    //SelectorPanel.GetGroupsFrom(cueRow.Groups);
                    Visibility = Visibility.Visible;
                }
            }
        }

        bool[] AvailableMotors(CueRow current) {
            var cue = DataContext as Cue;
            var ret = Enumerable.Repeat(true, 16).ToArray();
            foreach (var row in cue.Rows.Except(new CueRow[] { current })) {
                for (int i = 0; i < ret.Length; i++) ret[i] &= !row.Motors[i];
            }
            return ret;
        }

        bool[] AvailableGroups(CueRow current) {
            var cue = DataContext as Cue;
            var ret = Enumerable.Repeat(true, 8).ToArray();
            foreach (var row in cue.Rows.Except(new CueRow[] { current })) {
                for (int i = 0; i < ret.Length; i++) ret[i] &= !row.Groups[i];
            }
            return ret;
        }
        public class MaGEventArgs : EventArgs {
            public string Name { get; set; }
            public MotAndGroUI DataItem { get; set; }
        }

        public event EventHandler<MaGEventArgs> MaGEvent;

        private void Button_Click(object sender, RoutedEventArgs e) { 
            var but = sender as Button;
            if (but.Name == "SAVE") {
                var cueRow = dataItem.DataContext as CueRow;
                var sdc = SelectorPanel.DataContext as MaGSelectorDataContextType;
                Array.Copy(sdc.SelectedMotors, cueRow.Motors, cueRow.Motors.Length);
                Array.Copy(sdc.SelectedGroups, cueRow.Groups, cueRow.Groups.Length);
                cueRow.Validate();
            }

            MaGEvent?.Invoke(this, new MaGEventArgs { Name = but.Name, DataItem = DataItem });
        }
    }
}
