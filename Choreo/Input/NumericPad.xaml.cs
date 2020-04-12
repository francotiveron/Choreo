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
using System.Windows.Shapes;

namespace Choreo.Input {
    /// <summary>
    /// Interaction logic for NumericPad.xaml
    /// </summary>
    public partial class NumericPad : Pad {
        public NumericPad() {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e) {
            Button button = sender as Button;
            var cmd = button.CommandParameter.ToString();

            switch (cmd) {
                case "ESC":
                case "RETURN":
                case "BACK":
                    OnPadEvent(cmd);
                    break;

                default:
                    OnPadEvent(button.Content.ToString());
                    break;
            }
        }
    }
}
