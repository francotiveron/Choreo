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
            if (e.OriginalSource is DataItemUI diui)
                NumPad.DataItem = diui;
            e.Handled = true;
        }

        //public static void SetEditingItem(object item) {
        //    switch(item) {
        //        case Motor m: VM.MotorSettingsBeingEdited = m.Index + 1; break;
        //    }
        //}

        //private void ResetGroupButton_Click(object sender, RoutedEventArgs e) {
        //    VM.Motors[VM.MotorSettingsBeingEdited - 1].Group = 0;
        //}

        private void NumPad_PadEvent(object sender, Input.NumericPad1.PadEventArgs e) => FocusManager.SetFocusedElement(EditableElementsGrid, e.DataItem.Navigate(e.Name));

}

    //public class EditDataItemSetter: DynamicObject {
    //    public override bool TryInvoke(InvokeBinder binder, object[] args, out object result) {
    //        MotorPage.SetEditingItem(args[0]);
    //        result = null;
    //        return true;
    //    }
    //}
}
