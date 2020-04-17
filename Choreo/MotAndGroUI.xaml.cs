using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace Choreo {
    /// <summary>
    /// Interaction logic for MotAndGro.xaml
    /// </summary>
    public partial class MotAndGroUI : UserControl {
        public MotAndGroUI() {
            InitializeComponent();

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            if (DataItemUI.ValueFontSize != null) {
                Motors.SetValue(FontSizeProperty, DataItemUI.ValueFontSize);
                Groups.SetValue(FontSizeProperty, DataItemUI.ValueFontSize);
            }
            if (DataItemUI.LabelFontSize != null) Label.SetValue(FontSizeProperty, DataItemUI.LabelFontSize);
        }
    }

    public class MotAndGroConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var res = "???";

            if (value is Motion motion) {
                (bool isGroup, int index) hook = (false, 0);

                string ConvertBoolArray(bool[] a, int? except) {
                    var l = new List<string>();
                    for(int i = 0; i < a.Length; i++) {
                        if (i == except || !a[i]) continue;
                        l.Add((i + 1).ToString("00"));
                    }
                    return string.Join(",", l.ToArray());
                }

                switch (motion.Hook) {
                    case Motor m:
                        hook = (false, m.Index);
                        break;
                    case Group g:
                        hook = (true, g.Index);
                        break;
                }

                switch (parameter) {
                    case "Motors":
                        res = $"M:{ConvertBoolArray(motion.Motors, hook.isGroup ? (int?)null : hook.index)}";
                        break;
                    case "Groups":
                        res = $"G:{ConvertBoolArray(motion.Groups, hook.isGroup ? hook.index : (int?)null)}";
                        break;
                }
            }
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
