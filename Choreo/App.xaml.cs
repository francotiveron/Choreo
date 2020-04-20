using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using static Choreo.Globals;

namespace Choreo
{
    public partial class App : Application
    {
        public App() {
            VM.PropertyChanged += VM_PropertyChanged; ;
        }
        private void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == "MotorSettingsBeingEdited") {
                var motorIndex = VM.MotorSettingsBeingEdited;
                if (motorIndex > 0 && MainWindow is MainWindow) {
                    var newWindow = new MotorWindow();
                    newWindow.DataContext = VM.Motors[motorIndex - 1];
                    ChangeWindow(newWindow);
                }
                else
                if (motorIndex == 0 && MainWindow is MotorWindow) ChangeWindow(new MainWindow());
            }
            else
            if (e.PropertyName == "GroupSettingsBeingEdited") {
                var groupIndex = VM.GroupSettingsBeingEdited;
                if (groupIndex > 0 && MainWindow is MainWindow) {
                    var newWindow = new MotorWindow();
                    newWindow.DataContext = VM.Groups[groupIndex - 1];
                    ChangeWindow(newWindow);
                }
                else
                if (groupIndex == 0 && MainWindow is MotorWindow) ChangeWindow(new MainWindow());
            }
            else
            if (e.PropertyName == "CueBeingEdited") {
                var cueIndex = VM.CueBeingEdited;
                if (cueIndex > 0 && MainWindow is MainWindow) {
                    var newWindow = new CueWindow();
                    newWindow.DataContext = VM.Cues[cueIndex - 1];
                    ChangeWindow(newWindow);
                }
                else
                if (cueIndex == 0 && MainWindow is CueWindow) ChangeWindow(new MainWindow());
            }
            else
            if (e.PropertyName == "MotionEditing") {
                var cueIndex = VM.CueBeingEdited;
                if (VM.MotionEditing && MainWindow is MainWindow) {
                    var newWindow = new MotionWindow();
                    ChangeWindow(newWindow);
                }
                else
                if (!VM.MotionEditing && MainWindow is MotionWindow) ChangeWindow(new MainWindow());
            }
        }

        private void ChangeWindow(Window newWindow) {
            var oldWindow = MainWindow;
            MainWindow = newWindow;
            newWindow.Show();
            oldWindow.Close();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            PresentationTraceSources.Refresh();
            PresentationTraceSources.DataBindingSource.Listeners.Add(new ConsoleTraceListener());
            PresentationTraceSources.DataBindingSource.Listeners.Add(new DebugTraceListener());
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Warning | SourceLevels.Error;
            SetupExceptionHandling();
            MainWindow = new MainWindow();
            MainWindow.Show();
        }
        private void SetupExceptionHandling() {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            DispatcherUnhandledException += (s, e) =>
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

        private void LogUnhandledException(Exception exception, string source) {
            string message = $"Unhandled exception ({source})";
            try {
                System.Reflection.AssemblyName assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
                message = string.Format("Unhandled exception in {0} v{1}\nStackTrace:\n{2}", assemblyName.Name, assemblyName.Version, exception.StackTrace);
            }
            catch (Exception ex) {
                Logger.Error(ex, "Exception in LogUnhandledException");
            }
            finally {
                Logger.Error(exception, message);
            }
        }
    }

    public class DebugTraceListener : TraceListener
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
