using System.Windows;
using System.Windows.Controls;

namespace Choreo
{
    public class ModeableListView : UserControl {
        public bool IsEditable {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Editable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(bool), typeof(ModeableListView), new PropertyMetadata(false));
    }
}
