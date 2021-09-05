using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Choreo.Input
{
    public interface IStrVal {
        string StrVal { get; set; }
        bool IsPassword { get; }
    }
    public partial class AlphaNumericPad : UserControl {
        public AlphaNumericPad() {
            InitializeComponent();
            DataContext = this;
            Visibility = DataItem == null ? Visibility.Hidden : Visibility.Visible;
            ToggleCapsLock();
        }

        public string Value {
            get => DataItem.IsPassword ? PasswordTextBox.Password : ValueTextBox.Text;
            set { if (DataItem.IsPassword) PasswordTextBox.Password = value; else ValueTextBox.Text = value; }
        }

        IStrVal dataItem;
        bool isNew;
        public IStrVal DataItem {
            get => dataItem;
            set {
                dataItem = value;
                if (dataItem == null) Visibility = Visibility.Hidden;
                else {
                    if (dataItem.IsPassword) {
                        PasswordTextBox.Visibility = Visibility.Visible;
                        ValueTextBox.Visibility = Visibility.Hidden;
                    }
                    else {
                        PasswordTextBox.Visibility = Visibility.Hidden;
                        ValueTextBox.Visibility = Visibility.Visible;
                    }
                    Visibility = Visibility.Visible;
                    Value = dataItem.StrVal;
                    ValueTextBox.Focus();
                    ValueTextBox.SelectAll();
                    isNew = true;
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
                    AlNumEvent?.Invoke(this, new AlNumEventArgs { Name = but.Name, DataItem = DataItem });
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
                    if (but.Name.StartsWith("NUM") || but.Name.StartsWith("ALP"))
                        if (isNew && ValueTextBox.SelectionLength > 0) Value = (string)but.Content;
                        else Value += but.Content;
                    break;
            }
            isNew = false;
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
