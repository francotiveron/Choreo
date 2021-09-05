using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static Choreo.Globals;

namespace Choreo
{
    public partial class App : Application
    {
        public App() {
            InitializeComponent();
            VM.PropertyChanged += VM_PropertyChanged;
            QuickConverter.EquationTokenizer.AddNamespace(typeof(object));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(Visibility));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(Brush));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(CheckBox));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(Label));
        }

        void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
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

        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    base.OnStartup(e);
        //    //PresentationTraceSources.Refresh();
        //    //PresentationTraceSources.DataBindingSource.Listeners.Add(new ConsoleTraceListener());
        //    //PresentationTraceSources.DataBindingSource.Listeners.Add(new DebugTraceListener());
        //    //PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Warning | SourceLevels.Error;
        //    //SetupExceptionHandling();
        //    MainWindow = new MainWindow();
        //    MainWindow.Show();
        //}
    }
}
