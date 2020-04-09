using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Choreo.Globals;

namespace Choreo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RoutedCommand HomeCmd = new RoutedCommand();
        public static RoutedCommand CueingCmd = new RoutedCommand();
        public static RoutedCommand ShowCmd = new RoutedCommand();

        public MainWindow()
        {
            InitializeComponent();
            SelectPage();
            VM.PropertyChanged += VM_PropertyChanged;
        }

        private void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == "GroupBeingEdited") {
                var group = VM.GroupBeingEdited;
                if (group > 0) {
                    if (!(TopPanelArea.Child is GroupTopPanel)) TopPanelArea.Child = new GroupTopPanel();
                    var panel = (GroupTopPanel)TopPanelArea.Child;
                    if (panel.DataContext != VM.Groups[group - 1]) panel.DataContext = VM.Groups[group - 1];
                }
                else TopPanelArea.Child = new HomeTopPanel();
            }
            else
            if (e.PropertyName == "PresetBeingEdited") {
                var preset = VM.PresetBeingEdited;
                if (preset > 0) {
                    if (!(TopPanelArea.Child is PresetTopPanel)) TopPanelArea.Child = new PresetTopPanel();
                    var panel = (PresetTopPanel)TopPanelArea.Child;
                    if (panel.DataContext != VM.Presets[preset - 1]) panel.DataContext = VM.Presets[preset - 1];
                }
                else TopPanelArea.Child = new HomeTopPanel();
            }
            else
            if (e.PropertyName == "CurrentMainWindowPage")
                SelectPage();
        }

        void SelectPage() {
            switch(VM.CurrentMainWindowPage) {
                case MainWindowPages.Home:
                    PageArea.Child = new HomePage();
                    break;

                case MainWindowPages.Cueing:
                    PageArea.Child = new CueingPage();
                    break;

                case MainWindowPages.Show:
                    PageArea.Child = new ShowPage();
                    break;
            }
        }

        private void HomeCmdExecuted(object sender, ExecutedRoutedEventArgs e) => VM.CurrentMainWindowPage = MainWindowPages.Home;
        private void HomeCmdCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = VM.CurrentMainWindowPage != MainWindowPages.Home;
        private void CueingCmdExecuted(object sender, ExecutedRoutedEventArgs e) => VM.CurrentMainWindowPage = MainWindowPages.Cueing;
        private void CueingCmdCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = VM.CurrentMainWindowPage != MainWindowPages.Cueing;
        private void ShowCmdExecuted(object sender, ExecutedRoutedEventArgs e) => VM.CurrentMainWindowPage = MainWindowPages.Show;
        private void ShowCmdCanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = VM.CurrentMainWindowPage != MainWindowPages.Show;
    }
}
