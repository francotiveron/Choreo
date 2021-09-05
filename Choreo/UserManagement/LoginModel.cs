namespace Choreo.UserManagement
{
    public class LoginModel : PropertyChangedNotifier {
		string username = string.Empty;
		[DataItem]
		public string Username {
			get => username;
			set { username = value ?? string.Empty; Notify(); }
		}

		string password = string.Empty;
		[DataItem]
		public string Password {
			get => password;
			set { password = value ?? string.Empty; Notify(); }
		}
	}
}