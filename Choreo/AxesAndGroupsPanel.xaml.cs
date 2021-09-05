using System.Windows.Controls;
using static Choreo.Globals;

namespace Choreo
{
    /// <summary>
    /// Interaction logic for AxisCentralPanel.xaml
    /// </summary>
    public partial class AxesAndGroupsPanel : UserControl
    {
        public AxesAndGroupsPanel()
        {
            InitializeComponent();
        }
        private void JogVelSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e) => UpdateJogVelocity();
        private void JogVelSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e) => UpdateJogVelocity();

        private void UpdateJogVelocity() => Plc.Upload(JogVelSlider.Value);

        private void ClearButton_Click(object sender, System.Windows.RoutedEventArgs e) {
            Plc.ClearMotionAndJog();
            Plc.Upload(default(Preset));
        }
    }
}
