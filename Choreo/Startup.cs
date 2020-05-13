using Choreo.UserManagement;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static Choreo.Globals;
using Cfg = Choreo.Configuration;

namespace Choreo
{
    class Startup
    {
        [STAThread]
        static void Main(string[] args)
        {
            Debugger.Launch();
            Cfg.ParseCommandLine(args);

            PresentationTraceSources.Refresh();
            PresentationTraceSources.DataBindingSource.Listeners.Add(new ConsoleTraceListener());
            PresentationTraceSources.DataBindingSource.Listeners.Add(new DebugTraceListener());
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Warning | SourceLevels.Error;

            App app = new App();
            SetupExceptionHandling(app);
            app.MainWindow = new MainWindow();

            User.Init();

            app.MainWindow.Show();
            app.Run();
        }
        private static void SetupExceptionHandling(Application app)
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            app.DispatcherUnhandledException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
        }

        private static void LogUnhandledException(Exception exception, string source)
        {
            string message = $"Unhandled exception ({source})";
            try
            {
                System.Reflection.AssemblyName assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
                message = string.Format("Unhandled exception in {0} v{1}\nStackTrace:\n{2}", assemblyName.Name, assemblyName.Version, exception.StackTrace);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception in LogUnhandledException");
            }
            finally
            {
                Log.Error(exception, message);
            }
        }
    }
    class DebugTraceListener : TraceListener
    {
        public override void Write(string message)
        {
        }

        public override void WriteLine(string message)
        {
            //Debugger.Break();
        }
    }
}
