using System.Windows;
using System.Windows.Controls;
using static Choreo.Globals;

namespace Choreo
{
    /// <summary>
    /// Interaction logic for GroupTopPanel.xaml
    /// </summary>
    public partial class GroupTopPanel : UserControl {
        public GroupTopPanel() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            var button = (Button)sender;
            switch(button.Name) {
                case "GroupEditSaveButton":
                    VM.GroupEditSave();
                    break;
                case "GroupEditCancelButton":
                    VM.GroupEditCancel();
                    break;
                case "GroupEditClearButton":
                    VM.GroupEditClear();
                    break;
            }
        }
    }
}
