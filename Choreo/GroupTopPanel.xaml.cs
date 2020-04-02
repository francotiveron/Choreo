using System;
using System.Collections.Generic;
using System.Globalization;
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
using TwinCAT.TypeSystem;
using static Choreo.Globals;

namespace Choreo {
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
