using System.Windows;

namespace Choreo
{
    /// <summary>
    /// Interaction logic for MotionWindow.xaml
    /// </summary>
    public partial class MotionWindow : Window {
        public MotionWindow() {
            InitializeComponent();
            DataItemUI.ValueFontSize = Resources["DataItemValueFontSize"];
            DataItemUI.LabelFontSize = Resources["DataItemLabelFontSize"];
        }
    }
}
