using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using static Choreo.Globals;
using Cfg = Choreo.Configuration;

namespace Choreo.UserManagement
{
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

        public static void Init() {
            if (!Cfg.UserManagement) return;
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            AppDomain.CurrentDomain.SetThreadPrincipal(principal = new ChoreoPrincipal());
        }

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

        class ChoreoPrincipal : IPrincipal {
            public ChoreoPrincipal() => Set(Thread.CurrentPrincipal, Cfg.AutoLogin);
            
            IPrincipal principal;
            void Set(IPrincipal principal, bool fetchRole) {
                this.principal = principal;
                if (fetchRole) Role = FetchHighestRole(principal);
                VM.IsAdmin = IsAdmin;
                
            }
            public void Set(IPrincipal principal) => Set(principal, true);
            public void Reset() {
                principal = null;
                Role = null;
                VM.IsAdmin = IsAdmin;
            }
            public IIdentity Identity => principal.Identity;
            public bool IsInRole(string role) => principal.IsInRole(role);
            public bool IsAuthorised(Roles role) => Role.HasValue && role <= Role.Value;
            public Roles? Role { get; private set; }
            static Roles? FetchHighestRole(IPrincipal principal) {
                for (int i = roles.Length - 1; i >= 0; i--) {
                    var role = userGroups[roles[i]];
                    if (principal.IsInRole(role)) return roles[i];
                }
                return null;
            }

            public static bool Check(IPrincipal principal, Roles role) {
                var highestRole = FetchHighestRole(principal);
                return highestRole >= role;
            }
        }

        static ChoreoPrincipal principal;
        public static bool IsAdmin => principal?.IsAuthorised(Roles.Admin) ?? false;
        static bool Login(Roles? role) {
            var form = new LoginForm(principal => role.HasValue ? ChoreoPrincipal.Check(principal, role.Value) : true);
            var success = form.ShowDialog();
            if (success != true) return false;
            principal.Set(form.Principal);
            return true;
        }

        public static void Login() => Login(null);

        internal static void Logout() => principal.Reset();


        public enum Roles { Limited, Normal, Power, Admin };
        static Roles[] roles = (Roles[])Enum.GetValues(typeof(Roles));
        static Dictionary<Roles, string> userGroups = new Dictionary<Roles, string> {
            { Roles.Limited, "ChoreoLimitedUsers" }
            , { Roles.Normal, "ChoreoUsers" }
            , { Roles.Power, "ChoreoPowerUsers" }
            , { Roles.Admin, "ChoreoAdministrators" }
        };

        public static void Require(Roles role) {
            if (!Cfg.UserManagement) return;
            if (principal.IsAuthorised(role)) return;
            if (Login(role)) return;
            throw new ChoreoUserException();
        }

        public static void RequireLimited() => Require(Roles.Limited);
        public static void RequireNormal() => Require(Roles.Normal);
        public static void RequirePower() => Require(Roles.Power);
        public static void RequireAdmin() => Require(Roles.Admin);
    }

    public class ChoreoUserException : Exception { }
}
