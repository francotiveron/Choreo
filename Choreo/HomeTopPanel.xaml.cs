using Choreo.UserManagement;
using System;
using System.Linq;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using static Choreo.Globals;

namespace Choreo
{
    /// <summary>
    /// Interaction logic for MainTopPanel.xaml
    /// </summary>
    public partial class HomeTopPanel : UserControl
    {
        public HomeTopPanel()
        {
            InitializeComponent();
        }
        private void ExitItem_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void LoginItem_Click(object sender, RoutedEventArgs e) => User.Login();
        private void LogoutItem_Click(object sender, RoutedEventArgs e) => User.Logout();

        private void UsersItem_Click(object sender, RoutedEventArgs e) {
            //System.Diagnostics.Process.Start("C:\\Windows\\System32\\dsa.msc");
            bool ok = Log.OkCancel(
                "Choreo User Management is integrated with Windows. " +
                "Choreo will close and the Windows Local User Management snap-in will open. " +
                "Users must be created as normal Windows user and added to one of the the following groups:\n\n" +
                "ChoreoLimitedUsers, ChoreoUsers, ChoreoPowerUsers, CoreoAdministrators\n\n" +
                "To activate the changes a Windows logout/relogin is required, then Choreo can be restarted. " +
                "Please note that you need to be granted the appropriate Windows credentials to change users."
                , "Users Management"
                );
            if (!ok) return;
            System.Diagnostics.Process.Start("lusrmgr.msc");
            Application.Current.Shutdown();
        }
        private void ClearButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Plc.ClearMotion();
            Plc.Upload(default(Preset));
        }

        private void MainMenu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var left = e.ChangedButton == MouseButton.Left;
            var right = e.ChangedButton == MouseButton.Right;

            if (right || Configuration.UserManagement)
            {
                ContextMenu contextMenu = MainMenu.ContextMenu;

                foreach (var mi in contextMenu.Items.OfType<MenuItem>())
                {
                    switch (mi.Name)
                    {
                        case "ExitItem": mi.Visibility = right ? Visibility.Visible : Visibility.Collapsed; break;
                        case "UsersItem": mi.Visibility = left && VM.IsAdmin && Configuration.UserManagement ? Visibility.Visible : Visibility.Collapsed; break;
                        default: mi.Visibility = left && Configuration.UserManagement ? Visibility.Visible : Visibility.Collapsed; break;
                    }
                }

                contextMenu.PlacementTarget = MainMenu;
                contextMenu.IsOpen = true;
            }

            e.Handled = true;
        }
    }
    public class CurrentPageVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString().Equals(parameter) ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
