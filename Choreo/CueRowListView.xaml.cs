using System.Windows;
using System.Windows.Controls;
using static Choreo.Globals;

namespace Choreo {
    /// <summary>
    /// Interaction logic for CueRowListView.xaml
    /// </summary>
    public partial class CueRowListView : UserControl {
        public CueRowListView() {
            InitializeComponent();
        }
        private void DeleteRowButton_Click(object sender, RoutedEventArgs e) {
            var button = sender as Button;
            var row = button.DataContext as CueRow;
            VM.DeleteCueRow(row.Index);
            e.Handled = true;
        }

        private void FieldLabel_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            //var label = sender as Label;
            //var x = label.GetBindingExpression(Label.ContentProperty);

            Edit(sender);
        }
        //{
            //var label = sender as Label;
            //var field = label.Name;
            //Window pad = null;
            //switch(field) {
            //    case "Target":
            //        pad = new Keypad

            //}
        //}
    }
}
