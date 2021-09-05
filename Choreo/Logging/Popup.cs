using System.Windows;

namespace Choreo.Logging
{
    public abstract class Popup: Window {
        public Popup() {
            Owner = Application.Current.MainWindow;
            ResizeMode = ResizeMode.NoResize;
            ShowInTaskbar = false;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            WindowStyle = WindowStyle.None;
        }
    }
}
