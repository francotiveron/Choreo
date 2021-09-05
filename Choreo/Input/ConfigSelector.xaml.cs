using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using static Choreo.Globals;
using static Choreo.Input.AlphaNumericPad;

namespace Choreo.Input
{
    /// <summary>
    /// Interaction logic for ConfigSelector.xaml
    /// </summary>
    public partial class ConfigSelector : UserControl
    {
        public ConfigSelector()
        {
            InitializeComponent();
            Enable();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Enable();
        }

        public string[] Items {
            get
            {
                return Combo.ItemsSource.Cast<string>().ToArray();
            } 
            set
            {
                Combo.ItemsSource = value;
                Combo.SelectedIndex = -1;
            }
        }

        private void LoadButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var name = (string)Combo.SelectedItem;
            CfgSelEvent?.Invoke(this, new CfgSelEventArgs { Name = name });
        }

        private void SaveButton_Click(object sender, System.Windows.RoutedEventArgs e)
{
            CfgSelEvent?.Invoke(this, null);
        }
        private void DeleteButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var name = (string)Combo.SelectedItem;
            CfgSelEvent?.Invoke(this, new CfgSelEventArgs { Name = $"-{name}" });
        }

        public bool CanLoad
        {
            get { return (bool)GetValue(CanLoadProperty); }
            set { SetValue(CanLoadProperty, value); }
        }

        public static readonly DependencyProperty CanLoadProperty = DependencyProperty.Register("CanLoad", typeof(bool), typeof(ConfigSelector));

        void Enable()
        {
            DeleteButton.IsEnabled = Combo.SelectedIndex >= 0;
            LoadButton.IsEnabled = CanLoad && DeleteButton.IsEnabled;
        }

        public class CfgSelEventArgs : EventArgs
        {
            public string Name { get; set; }
        }
        public event EventHandler<CfgSelEventArgs> CfgSelEvent;
    }
}
