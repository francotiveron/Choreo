using System;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Choreo.Globals;

namespace Choreo {
    /// <summary>
    /// Interaction logic for MotorSettingsPage.xaml
    /// </summary>
    public partial class MotorPage : UserControl {
        public MotorPage() {
            InitializeComponent();
            FocusManager.AddGotFocusHandler(this, Focus);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) => FocusManager.SetFocusedElement(EditableElementsGrid, SetPosition);
        private void Focus(object sender, RoutedEventArgs e) {
            if (e.OriginalSource is DataItemUI diui) {
                if (diui == AxisName) {
                    NumPad.DataItem = null;
                    AlNumPad.DataItem = diui;
                }
                else {
                    NumPad.DataItem = diui;
                    AlNumPad.DataItem = null;
                }
            }
            e.Handled = true;
        }
        private void NumPad_PadEvent(object sender, Input.NumericPad1.PadEventArgs e) => FocusManager.SetFocusedElement(EditableElementsGrid, e.DataItem.Navigate(e.Name));

        private void AlNumPad_AlNumEvent(object sender, Input.AlphaNumericPad.AlNumEventArgs e) {
            FocusManager.SetFocusedElement(EditableElementsGrid, null);
            AlNumPad.DataItem = null;
        }

        private void SaveNewPositionButton_Click(object sender, RoutedEventArgs e) => Plc.Calibrate(DataContext as Axis);
    }
}
