﻿using System.ComponentModel;
using System.Windows;
using static Choreo.Globals;

namespace Choreo
{
    /// <summary>
    /// Interaction logic for MotorSettingsWindow.xaml
    /// </summary>
    public partial class MotorWindow : Window {
        public MotorWindow() {
            InitializeComponent();
            DataItemUI.ValueFontSize = Resources["DataItemValueFontSize"];
            DataItemUI.LabelFontSize = Resources["DataItemLabelFontSize"];
            VM.PropertyChanged += VM_PropertyChanged;
        }

        protected override void OnClosing(CancelEventArgs e) {
            VM.PropertyChanged -= VM_PropertyChanged;
            base.OnClosing(e);
        }

        private void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == "MotorSettingsBeingEdited" && VM.IsMotorSettingsEditing) DataContext = VM.Motors[VM.MotorSettingsBeingEdited - 1];
        }
    }
}
