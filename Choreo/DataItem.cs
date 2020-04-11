using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Choreo {
	public class StatusColorConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return Colors.Yellow;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}

    [System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    sealed class DataItemAttribute : Attribute {
		public DataItemAttribute(string mu = null, string title = null, bool edit = false) {
			Title = title;
			MU = mu;
			Edit = edit;
		}

		public string Title { get; private set; }
		public string MU { get; private set; }
		public bool Edit { get; private set; }
	}
}
