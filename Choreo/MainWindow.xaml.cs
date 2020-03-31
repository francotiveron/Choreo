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
            DataContext = VM;
        }

        private void HomeCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            VM.CurrentPage = Pages.Home;
            PageArea.Child = new HomePage();
        }

        private void HomeCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = VM.CurrentPage != Pages.Home;
        }

        private void CueingCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            VM.CurrentPage = Pages.Cueing;
            PageArea.Child = new CueingPage();
        }

        private void CueingCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = VM.CurrentPage != Pages.Cueing;
        }

        private void ShowCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            VM.CurrentPage = Pages.Show;
            PageArea.Child = new ShowPage();
        }

        private void ShowCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = VM.CurrentPage != Pages.Show;
        }
    }
}
