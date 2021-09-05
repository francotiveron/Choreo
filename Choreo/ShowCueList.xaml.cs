using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Choreo
{
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

    public class LoadedCueVisibilityConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            var cue = values[0] as Cue;
            var loadedCue = (int)values[1];
            return cue.Number == loadedCue ? Visibility.Visible : Visibility.Hidden;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
