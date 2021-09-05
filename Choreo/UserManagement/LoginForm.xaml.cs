using System;
using System.Security.Principal;
using System.Windows;
using System.Windows.Input;

namespace Choreo.UserManagement
{
    public partial class LoginForm : Window {
        Func<IPrincipal, bool> roleChecker;
        public LoginForm(Func<IPrincipal, bool> checker) {
            roleChecker = checker;
            InitializeComponent();
            FailedLoginLabel.Visibility = Visibility.Hidden;
            FocusManager.AddGotFocusHandler(this, Focus);
        }

        private void Focus(object sender, RoutedEventArgs e) {
            if (e.OriginalSource is DataItemUI diui)
                AlNumPad.DataItem = diui;

            e.Handled = true;
        }

        private void AlNumPad_AlNumEvent(object sender, Input.AlphaNumericPad.AlNumEventArgs e) {
            AlNumPad.DataItem = null;
        }

        public IPrincipal Principal { get; private set; }

        private void LoginButton_Click(object sender, RoutedEventArgs e) {
            var login = DataContext as LoginModel;
            if (User.Login(login.Username, login.Password) is IPrincipal principal) {
                if (roleChecker(principal)) {
                    DialogResult = true;
                    Principal = principal;
                    Close();
                    return;
                }
                FailedLoginLabel.Content = "User Level is Insufficient";
            }
            else FailedLoginLabel.Content = "Login Failed";

            FailedLoginLabel.Visibility = Visibility.Visible;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
            Close();
        }
    }
}
