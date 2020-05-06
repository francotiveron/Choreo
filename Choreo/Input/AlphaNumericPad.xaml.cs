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
    public interface IStrVal {
        string StrVal { get; set; }
    }
    public partial class AlphaNumericPad : UserControl {
        public AlphaNumericPad() {
            InitializeComponent();
            DataContext = this;
            Visibility = DataItem == null ? Visibility.Hidden : Visibility.Visible;
            ToggleCapsLock();
        }

        public string Value {
            get => ValueTextBox.Text;
            set => ValueTextBox.Text = value;
        }

        IStrVal dataItem;
        public IStrVal DataItem {
            get => dataItem;
            set {
                dataItem = value;
                if (dataItem == null) Visibility = Visibility.Hidden;
                else {
                    Visibility = Visibility.Visible;
                    Value = dataItem.StrVal;
                }
            }
        }

        public class AlNumEventArgs : EventArgs {
            public string Name { get; set; }
            public IStrVal DataItem { get; set; }
        }

        public event EventHandler<AlNumEventArgs> AlNumEvent;
        private void Button_Click(object sender, RoutedEventArgs e) {
            var but = sender as Button;

            switch (but.Name) {
                case "ESC":
                    AlNumEvent?.Invoke(this, new AlNumEventArgs { Name = but.Name, DataItem = DataItem }); ;
                    break;
                case "CAPSLOCK":
                    ToggleCapsLock();
                    break;
                case "RST":
                    Value = DataItem.StrVal;
                    break;
                case "CLR":
                    Value = string.Empty;
                    break;
                case "BKSP":
                    Value = Value.Substring(0, Math.Max(Value.Length - 1, 0));
                    break;
                case "SPC":
                    Value += " ";
                    break;
                case "ENTER":
                    if (DataItem != null) DataItem.StrVal = Value;
                    AlNumEvent?.Invoke(this, new AlNumEventArgs { Name = but.Name, DataItem = DataItem }); ;
                    break;
                default:
                    if (but.Name.StartsWith("NUM") || but.Name.StartsWith("ALP")) Value += but.Content;
                    break;
            }
        }

        void ToggleCapsLock() {
            IsCapsLock = !IsCapsLock;
            foreach(var button in MainGrid.Children.OfType<Button>().Where(b => b.Name.StartsWith("ALP"))) {
                var content = (string)button.Content;
                button.Content = IsCapsLock ? content.ToUpper() : content.ToLower();
            }
            CAPSLOCK.Background = IsCapsLock ? new SolidColorBrush(Colors.Blue) : null;
        }
        bool IsCapsLock { get; set; }
    }
}
