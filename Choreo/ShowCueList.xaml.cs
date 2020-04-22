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

namespace Choreo {
    /// <summary>
    /// Interaction logic for ShowCueList.xaml
    /// </summary>
    public partial class ShowCueList : UserControl {
        public ShowCueList() {
            InitializeComponent();
        }
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            SelectedCue = e.AddedItems.Count > 0 ? e.AddedItems[0] as Cue : null;
        }

        public Cue SelectedCue {
            get { return (Cue)GetValue(SelectedCueProperty); }
            set { SetValue(SelectedCueProperty, value); }
        }

        public static readonly DependencyProperty SelectedCueProperty =
            DependencyProperty.Register("SelectedCue", typeof(Cue), typeof(ShowCueList));
    }
}
