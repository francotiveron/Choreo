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

namespace Choreo.Input {
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
                    SelectorPanel.GetMotorsFrom(cueRow.Motors);
                    SelectorPanel.GetGroupsFrom(cueRow.Groups);
                    Visibility = Visibility.Visible;
                }
            }
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
                SelectorPanel.PutMotorsInto(cueRow.Motors);
                SelectorPanel.PutGroupsInto(cueRow.Groups);
            }
            else
            if (but.Name == "CANCEL") {
                var cueRow = dataItem.DataContext as CueRow;
                SelectorPanel.GetMotorsFrom(cueRow.Motors);
                SelectorPanel.GetGroupsFrom(cueRow.Groups);
            }
            MaGEvent?.Invoke(this, new MaGEventArgs { Name = but.Name, DataItem = DataItem });
        }
    }
}
