using CommandLine;
using System;
using System.Configuration;
using System.Runtime.InteropServices;
using TwinCAT.Ads;

namespace Choreo
{
    static class Configuration {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("kernel32.dll")]
        static extern bool FreeConsole();
        class CommandLineOptions {
            [Option("no-login", Hidden = true)]
            public bool NoLogin { get; set; }

            [Option("auto-login", HelpText = "Interactive User Automatic Login")]
            public bool AutoLogin { get; set; }

            [Option("auto-logout", HelpText = "User Automatic Timed Logout (min=1/default=10/max=30 min)")]
            public int AutoLogout { get; set; }
        }

        static CommandLineOptions options;
        public static void ParseCommandLine(string[] args) {
            Parser.Default.ParseArguments<CommandLineOptions>(args)
            .WithParsed(opt => options = opt)
            .WithNotParsed(_ => {
                Console.ReadLine();
                Environment.Exit(-1);
            });

            if (options.AutoLogout < 0) options.AutoLogout = 0;
            if (options.AutoLogout > 30) options.AutoLogout = 30;

            FreeConsole();
            //IntPtr h = Process.GetCurrentProcess().MainWindowHandle;
            //ShowWindow(h, 0);
        }

        public static bool UserManagement => !options.NoLogin;
        public static bool AutoLogin => options.AutoLogin;
        public static int AutoLogout => options.AutoLogout;
        public static AmsNetId PlcAmsNetId => Parse(AmsNetId.Parse);
        public static AmsPort PlcAmsPort => Parse((s) => (AmsPort)Enum.Parse(typeof(AmsPort), s));

        static T Parse<T>(Func<string, T> parser) {
            string typeName = typeof(T).Name;
            var @default = Properties.Settings.Default;

            try {
                return parser(ConfigurationManager.AppSettings[typeName]);
            }
            catch {
                var pi = @default.GetType().GetProperty(typeName);
                return (T)pi.GetValue(@default);
            }
        }
    }
}
