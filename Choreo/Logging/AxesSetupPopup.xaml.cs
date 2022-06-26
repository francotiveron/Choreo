using System;
using System.Linq;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Collections.Generic;


namespace Choreo.Logging
{
    /// <summary>
    /// Interaction logic for AlertPopup.xaml
    /// </summary>
    public partial class AxesSetupPopup : Popup {
        readonly Axis triggerAxis;
        readonly Button triggerButton;
        readonly Dictionary<(int, int), Button> buttons;
        //public AxesSetupPopup() : base() => InitializeComponent();
        public AxesSetupPopup(Axis trigger) : base()
        {
            InitializeComponent();
            triggerAxis = trigger;

            buttons =
                SelectionGrid.Children
                .OfType<Button>()
                .ToDictionary(b => (Grid.GetRow(b), Grid.GetColumn(b)));
            
            foreach(var kvp in buttons)
            {
                var (r, c) = kvp.Key;
                var b = kvp.Value;
                
                Axis axis = null;
                switch(r)
                {
                    case 0:
                        axis = Globals.VM.Motors[c];
                        break;
                    case 1:
                        axis = Globals.VM.Motors[8 + c];
                        break;
                    case 2:
                        axis = Globals.VM.Groups[c];
                        break;
                }

                if (axis.IsOperational)
                {
                    ((TextBlock)b.Content).Text = axis.FullName;
                    if (axis == triggerAxis)
                    {
                        triggerButton = b;
                        b.Background = Brushes.Green;
                    }
                    b.Visibility = Visibility.Visible;
                }
                else b.Visibility = Visibility.Hidden;
            }

        }

        public IEnumerable<int> Selected()
        {
            foreach (var kvp in buttons)
            {
                var (r, c) = kvp.Key;
                var b = kvp.Value;
                if (b.Background != null) yield return r * 8 + c;
            }
        }

        //public AxesSetupPopup(string message, string caption = null, Themes theme = Themes.Info, bool ok = false, bool cancel = false) : this() {
        //    OkButton.Visibility = ok ? Visibility.Visible : Visibility.Collapsed;
        //    CancelButton.Visibility = cancel ? Visibility.Visible : Visibility.Collapsed;
        //    DataContext = new { caption, message, theme };
        //}

        private void OkButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
            Close();
        }

        private void GridButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            //var row = Grid.GetRow(button);
            //var column = Grid.GetColumn(button);
            if (button != triggerButton) button.Background = button.Background == null ? Brushes.Blue : null;
            e.Handled = true;
        }
    }
}
