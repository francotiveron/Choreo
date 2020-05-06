using Choreo.Input;
using System.Windows;
using System.Windows.Controls;
using static Choreo.Globals;

namespace Choreo {
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class HomePage : UserControl
    {
        public HomePage()
        {
            InitializeComponent();
        }
        private void AlNumPad_AlNumEvent(object sender, Input.AlphaNumericPad.AlNumEventArgs e) {
            PresetAlNumPad.DataItem = null;
        }

        private void RenamePresetButton_Click(object sender, RoutedEventArgs e) {
            PresetAlNumPad.DataItem = new PresetNameSetter(VM.Presets[VM.PresetBeingEdited - 1]);
        }
    }
    public class PresetNameSetter : IStrVal {
        Preset preset;
        public PresetNameSetter(Preset preset) => this.preset = preset;
        public string StrVal { get => preset.Name; set => preset.Name = value; }
    }
}
