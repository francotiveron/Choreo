using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
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
            IsEnabled = DataItem != null;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            var but = sender as Button;

            switch(but.Name) {
                case "NEXT":
                case "PREV":
                    if (DataItem != null) StrVal = Value;
                    PadEvent?.Invoke(this, new PadEventArgs { Name = but.Name, DataItem = DataItem }); ;
                    break;
                case "BKSP":
                    Value = Value.Substring(0, Math.Max(Value.Length - 1, 0));
                    break;
                case "CLR":
                    Value = string.Empty;
                    break;
                case "RST":
                    Value = StrVal;
                    break;
                case "PM":
                    if (Value.Length > 0) {
                        if (Value[0] == '-') Value = Value.Substring(1);
                        else Value = $"-{Value}";
                    }
                    break;
                case "DOT":
                    if (!Value.Contains('.') && char.IsDigit(Value.Last())) Value = Value + '.';
                    break;
                case "FEET":
                    if (!Value.Contains("'") && DataItem.IsPosition && int.TryParse(Value, out _)) Value = Value + "'";
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
                if (dataItem == null) IsEnabled = false;
                else {
                    IsEnabled = true;
                    Value = StrVal;
                }
            }
        }
        string StrVal {
            get => dataItem.StrVal.Replace("\"", string.Empty);
            set => DataItem.StrVal = value;
        }
    }
}
