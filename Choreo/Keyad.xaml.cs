using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Choreo {
    /// <summary>
    /// Interaction logic for KeyPad.xaml
    /// </summary>
    public partial class Keypad : Window, INotifyPropertyChanged {
        DataItemUI diui;
        public Keypad(DataItemUI ui) {
            InitializeComponent();
            Owner = GetWindow(diui = ui);
            DataContext = this;
            Result = diui.FormattedValue;
        }

        public string Result {
            get { return (string)Tag; }
            private set { Tag = value; this.OnPropertyChanged("Result"); }
        }

        private void button_Click(object sender, RoutedEventArgs e) {
            Button button = sender as Button;
            switch (button.CommandParameter.ToString()) {
                case "ESC":
                    DialogResult = false;
                    break;

                case "RETURN":
                    DialogResult = true;
                    break;

                case "BACK":
                    if (Result.Length > 0)
                        Result = Result.Remove(Result.Length - 1);
                    break;

                default:
                    Result += button.Content.ToString();
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(String info) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
