using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Choreo.Globals;

namespace Choreo
{

    public partial class MotorPanel : UserControl
    {
        public MotorPanel()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var uc = (MotorPanel)sender;
            var row = (int)uc.GetValue(Grid.RowProperty);
            var col = (int)uc.GetValue(Grid.ColumnProperty);
            var index = row * 8 + col;
            DataContext = VM.Motors[index];
        }

        #region Gesture
        bool gesture;
        bool Gesture {
            get => gesture;
            set {
                gesture = value;
                Mouse.OverrideCursor = gesture ? Cursors.ScrollNS : null;
            }
        }
        double gestureStart;
        private void Border_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {

        }

        private void Border_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {

        }

        private void Border_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {

        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                StartGesture(e.GetPosition(this));
            }
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (Gesture)
            {
                var y = e.GetPosition(this).Y;
                if ((y - gestureStart) < -10.0) GestureUp();
                if ((y - gestureStart) > 10.0) GestureDn();
            }
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Gesture = false;
        }

        private void StartGesture(Point p)
        {
            gestureStart = p.Y;
            Gesture = true;
        }

        private void GestureUp()
        {
            Gesture = false;
        }

        private void GestureDn()
        {
            Gesture = false;
        }

        #endregion
    }
}
    