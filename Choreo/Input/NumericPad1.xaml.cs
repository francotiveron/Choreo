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
    /// Interaction logic for Pad1.xaml
    /// </summary>
    public partial class NumericPad1 : UserControl {
        public NumericPad1() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            var but = sender as Button;

            switch(but.Name) {
                case "NEXT":
                case "PREV":
                case "UP":
                case "DN":
                    PadEvent?.Invoke(this, new PadEventArgs { Name = but.Name, DataItem = DataItem }); ;
                    break;
                case "SAVE":
                    if (DataItem != null) DataItem.StrVal = Value;
                    break;
                case "PM":
                    if (Value.Length > 0) {
                        if (Value[0] == '-') Value = Value.Substring(1);
                        else Value = $"-{Value}";
                    }
                    break;
                case "DOT":
                    if (!Value.Contains('.')) Value = Value + '.';
                    break;
                default:
                    if (but.Name.StartsWith("NUM")) Value += but.Content;
                    break;
            }
            
        }

        public class PadEventArgs: EventArgs {
            public string Name { get; set; }
            public DataItemUI DataItem { get; set; }
        }

        public event EventHandler<PadEventArgs> PadEvent;

        public string Value { 
            get => ValueTextBox.Text;
            set => ValueTextBox.Text = value;
        }

        DataItemUI dataItem;
        public DataItemUI DataItem {
            get => dataItem;
            set {
                dataItem = value;
                Value = dataItem.StrVal;
            }
        }
    }
}
