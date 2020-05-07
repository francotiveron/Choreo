using System.Security.Principal;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using static Choreo.Globals;

namespace Choreo.UserManagement {
    public partial class LoginForm : Window {
        public LoginForm() {
            InitializeComponent();
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
                DialogResult = true;
                Principal = principal;
                Close();
                return;
            }

            Log.Alert($"Failed Login for user {login.Username}", "Login Failure");
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
            Close();
        }
    }
}
