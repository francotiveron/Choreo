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
        public static RoutedCommand CueingCmd = new RoutedCommand();
        public static RoutedCommand HomeCmd = new RoutedCommand();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel();
        }

        private void HomeCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PageArea.Child = new HomePage();
        }

        private void HomeCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CueingCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PageArea.Child = new CueingPage();
        }

        private void CueingCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

    }
}
