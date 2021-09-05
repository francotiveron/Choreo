using System.Windows;
using System.Windows.Controls;
using static Choreo.Globals;

namespace Choreo
{
    /// <summary>
    /// Interaction logic for MotionTopPanel.xaml
    /// </summary>
    public partial class MotionTopPanel : UserControl {
        public MotionTopPanel() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (sender == MotionCancelButton) VM.EndMotionEditing();
            else
            if (sender == MotionSaveButton) {
                var window = Application.Current.MainWindow as MotionWindow;
                var page = window?.Page;
                if (DataItemsValid(page.EditableElementsGrid.Children)) VM.SaveMotionEditing();
                else Log.PopInfo("Please insert valid data", "Invalid Data Entry");
            }
        }
    }
}
