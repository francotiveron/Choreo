using System.Windows;

namespace Choreo {
    /// <summary>
    /// Interaction logic for CueSettingsWindow.xaml
    /// </summary>
    public partial class CueWindow : Window {
        public CueWindow() {
            InitializeComponent();
            DataItemUI.ValueFontSize = Resources["DataItemValueFontSize"];
            DataItemUI.LabelFontSize = Resources["DataItemLabelFontSize"];
        }
    }
}
