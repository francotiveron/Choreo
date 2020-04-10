using System;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;
using static Choreo.Globals;

namespace Choreo {
    /// <summary>
    /// Interaction logic for MotorSettingsPage.xaml
    /// </summary>
    public partial class MotorPage : UserControl {
        public MotorPage() {
            InitializeComponent();
        }

        public static void SetEditingItem(object item) {
            switch(item) {
                case Motor m: VM.MotorBeingEdited = m.Index + 1; break;
            }
        }

        private void ResetGroupButton_Click(object sender, RoutedEventArgs e) {
            VM.Motors[VM.MotorBeingEdited - 1].Group = 0;
        }
    }

    public class EditDataItemSetter: DynamicObject {
        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result) {
            MotorPage.SetEditingItem(args[0]);
            result = null;
            return true;
        }
    }
}
