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
            if (e.PropertyName == "MotorBeingEdited") {
                var motorIndex = VM.MotorBeingEdited;
                if (motorIndex > 0 && MainWindow is MainWindow) {
                    var newWindow = new MotorWindow();
                    newWindow.DataContext = VM.Motors[motorIndex - 1];
                    ChangeWindow(newWindow);
                }
                else
                if (motorIndex == 0 && MainWindow is MotorWindow) ChangeWindow(new MainWindow());
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
        }

        private void ChangeWindow(Window newWindow) {
            var oldWindow = MainWindow;
            MainWindow = newWindow;
            newWindow.Show();
            oldWindow.Close();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow();
            MainWindow.Show();
            PresentationTraceSources.Refresh();
            PresentationTraceSources.DataBindingSource.Listeners.Add(new ConsoleTraceListener());
            PresentationTraceSources.DataBindingSource.Listeners.Add(new DebugTraceListener());
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Warning | SourceLevels.Error;
            base.OnStartup(e);
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
