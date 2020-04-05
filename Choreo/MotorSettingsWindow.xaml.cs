using System.Windows;
using static Choreo.Globals;

namespace Choreo {
    /// <summary>
    /// Interaction logic for MotorSettingsWindow.xaml
    /// </summary>
    public partial class MotorSettingsWindow : Window {
        public MotorSettingsWindow() {
            InitializeComponent();
            VM.PropertyChanged += VM_PropertyChanged;
        }
        private void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == "MotorSettingsBeingEdited" && VM.IsMotorSettingsEditing) DataContext = VM.Motors[VM.MotorSettingsBeingEdited - 1];
        }
    }
}
