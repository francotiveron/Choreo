using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;

namespace Choreo.UserManagement {
    public static class User {
        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_LOGON_NETWORK = 3;
        public const int LOGON32_LOGON_BATCH = 4;
        public const int LOGON32_LOGON_SERVICE = 5;
        public const int LOGON32_LOGON_UNLOCK = 7;
        public const int LOGON32_LOGON_NETWORK_CLEARTEXT = 8;
        public const int LOGON32_LOGON_NEW_CREDENTIALS = 9;

        public const int LOGON32_PROVIDER_DEFAULT = 0;
        public const int LOGON32_PROVIDER_WINNT35 = 1;
        public const int LOGON32_PROVIDER_WINNT40 = 2;
        public const int LOGON32_PROVIDER_WINNT50 = 3;

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern int LogonUserA(String lpszUserName,
            String lpszDomain,
            String lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int DuplicateToken(IntPtr hToken,
            int impersonationLevel,
            ref IntPtr hNewToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);
        [DllImport("ADVAPI32")]private unsafe static extern int LogonUser(String lpszUsername, String lpszDomain, String lpszPasswords, int dwLogonType, int dwLogonProvider, out IntPtr phToken);

        static User() => AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);

            //int ret = LogonUser("Pippo", ".", "Pluto", LOGON32_LOGON_NETWORK, LOGON32_PROVIDER_DEFAULT, out IntPtr token);
            ////WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
            //var id = new WindowsIdentity(token);
            //var pr = new WindowsPrincipal(id);
            //Thread.CurrentPrincipal = pr;
            //CloseHandle(token);

            //// Construct a GenericIdentity object based on the current Windows
            //// identity name and authentication type.
            //string authenticationType = windowsIdentity.AuthenticationType;
            //string userName = windowsIdentity.Name;
            //WindowsPrincipal myWindowsPrincipal = (WindowsPrincipal)Thread.CurrentPrincipal;

            //Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("Bob"), new string[] { "ChoreoUsers" });
        internal static void Init() {}
        internal static IPrincipal Login(string username, string password) {
            if (0 != LogonUser(username, ".", password, LOGON32_LOGON_NETWORK, LOGON32_PROVIDER_DEFAULT, out IntPtr token)) {
                try {
                    var id = new WindowsIdentity(token);
                    var pr = new WindowsPrincipal(id);
                    return pr;
                }
                catch { }
                finally { CloseHandle(token); }
            }

            return null;
        }
    }
}
