using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Choreo {
    /// <summary>
    /// Interaction logic for CueSettingsPage.xaml
    /// </summary>
    public partial class CuePage : UserControl {
        public CuePage() {
            InitializeComponent();
            FocusManager.AddGotFocusHandler(this, Focus);
        }
        private void Focus(object sender, RoutedEventArgs e) {
            if (e.OriginalSource is DataItemUI diui)
                NumPad.DataItem = diui;
            e.Handled = true;
        }

        private void NumPad_PadEvent(object sender, Input.NumericPad1.PadEventArgs e) => FocusManager.SetFocusedElement(EditableElementsGrid, e.DataItem.Navigate(e.Name));
    }
}
