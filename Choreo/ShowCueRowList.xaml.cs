using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for ShowCueRowList.xaml
    /// </summary>
    public partial class ShowCueRowList : UserControl {
        public ShowCueRowList() {
            InitializeComponent();
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
                    .Select(vi => vi.i.ToString())
                    .ToArray());
            }
            return $"{BoolArrToString(cueRow.Motors)}/{BoolArrToString(cueRow.Groups)}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
