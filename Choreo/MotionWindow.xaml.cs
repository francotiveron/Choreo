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
using System.Windows.Shapes;

namespace Choreo {
    /// <summary>
    /// Interaction logic for MotionWindow.xaml
    /// </summary>
    public partial class MotionWindow : Window {
        public MotionWindow() {
            InitializeComponent();
            DataItemUI.ValueFontSize = Resources["DataItemValueFontSize"];
            DataItemUI.LabelFontSize = Resources["DataItemLabelFontSize"];
        }
    }
}
