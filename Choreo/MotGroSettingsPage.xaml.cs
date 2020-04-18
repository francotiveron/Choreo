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
    public partial class MotGroSettingsPage : UserControl {
        public MotGroSettingsPage() {
            InitializeComponent();
        }

        public static void SetEditingItem(object item) {
            switch(item) {
                case Motor m: VM.MotorSettingsBeingEdited = m.Index + 1; break;
            }
        }

        private void ResetGroupButton_Click(object sender, RoutedEventArgs e) {
            VM.Motors[VM.MotorSettingsBeingEdited - 1].Group = 0;
        }
        private void NumPad_PadEvent(object sender, Input.NumericPad1.PadEventArgs e) {
            DataItemUI diui = null;

            switch (e.Name) {
                case "NEXT":
                    for (diui = e.DataItem.EditOrderNext; diui != e.DataItem && !diui.IsEnabled; diui = diui.EditOrderNext) ;
                    break;
                case "PREV":
                    for (diui = e.DataItem.EditOrderPrev; diui != e.DataItem && !diui.IsEnabled; diui = diui.EditOrderPrev) ;
                    break;
                case "UP":
                    for (diui = e.DataItem.EditOrderUp; diui != e.DataItem && !diui.IsEnabled; diui = diui.EditOrderUp) ;
                    break;
                case "DN":
                    for (diui = e.DataItem.EditOrderDn; diui != e.DataItem && !diui.IsEnabled; diui = diui.EditOrderDn) ;
                    break;
            }
            FocusManager.SetFocusedElement(EditableElementsGrid, diui);
        }
    }

    //public class EditDataItemSetter: DynamicObject {
    //    public override bool TryInvoke(InvokeBinder binder, object[] args, out object result) {
    //        MotorPage.SetEditingItem(args[0]);
    //        result = null;
    //        return true;
    //    }
    //}
}
