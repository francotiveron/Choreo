using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Choreo
{
    /// <summary>
    /// Interaction logic for CueSettingsPage.xaml
    /// </summary>
    public partial class CuePage : UserControl {
        public CuePage() {
            InitializeComponent();
            FocusManager.AddGotFocusHandler(this, Focus);
        }
        private void Focus(object sender, RoutedEventArgs e) {
            if (e.OriginalSource is DataItemUI diui) {
                if (diui == CueName) {
                    NumPad.DataItem = null;
                    AlNumPad.DataItem = diui;
                }
                else {
                    NumPad.DataItem = diui;
                    AlNumPad.DataItem = null;
                }
                MaGSelect.DataItem = null;
            }
            else
            if (e.OriginalSource is MotAndGroUI mgui) {
                NumPad.DataItem = null;
                AlNumPad.DataItem = null;
                MaGSelect.DataItem = mgui;
            }
            e.Handled = true;
        }

        private void NumPad_PadEvent(object sender, Input.NumericPad1.PadEventArgs e) => FocusManager.SetFocusedElement(EditableElementsGrid, e.DataItem.Navigate(e.Name));

        private void MaGSelect_MaGEvent(object sender, Input.MaGSelectionPopup.MaGEventArgs e) {
            FocusManager.SetFocusedElement(EditableElementsGrid, null);
            MaGSelect.DataItem = null;
            e.DataItem.Motors.GetBindingExpression(ContentProperty).UpdateTarget();
            e.DataItem.Groups.GetBindingExpression(ContentProperty).UpdateTarget();
        }

        private void AlNumPad_AlNumEvent(object sender, Input.AlphaNumericPad.AlNumEventArgs e) {
            FocusManager.SetFocusedElement(EditableElementsGrid, null);
            AlNumPad.DataItem = null;
        }
    }
}
