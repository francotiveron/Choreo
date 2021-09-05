using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Choreo
{
    /// <summary>
    /// Interaction logic for ComboPad.xaml
    /// </summary>
    public partial class SelectPad : Window {
        DataItemUI diui;
        public SelectPad(DataItemUI ui) {
            InitializeComponent();
            Owner = GetWindow(diui = ui);
            DataContext = this;
        }

        public IEnumerable Items { get; }
        private void button_Click(object sender, RoutedEventArgs e) {
            Button button = sender as Button;
            switch (button.CommandParameter.ToString()) {
                case "ESC":
                    DialogResult = false;
                    break;

                case "RETURN":
                    Tag = List.SelectedItem;
                    DialogResult = true;
                    break;
            }
        }
    }
}
