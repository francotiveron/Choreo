using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static Choreo.Globals;


namespace Choreo
{
    /// <summary>
    /// Interaction logic for ShowCueRowList.xaml
    /// </summary>
    public partial class ShowCueRowList : UserControl {
        public ShowCueRowList() {
            InitializeComponent();
        }

        private void Issues_Click(object sender, RoutedEventArgs e)
        {
            var row = (CueRow)((Control)sender).DataContext;
            Log.Pop(row.InconsistencyMessage, "Cue Row Issues", Logging.AlertPopup.Themes.Error, true, false);
        }
    }

    public class CueRowMaGToStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var cueRow = value as CueRow;

            string BoolArrToString(bool[] array) {
                return string.Join(",",
                    array
                    .Select((v, i) => (v, i))
                    .Where(vi => vi.v)
                    .Select(vi => (vi.i + 1).ToString())
                    .ToArray());
            }
            return $"{BoolArrToString(cueRow.Motors)}/{BoolArrToString(cueRow.Groups)}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
