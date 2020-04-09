using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
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
    public partial class DataItemUI {
        public DataItemUI() {
            InitializeComponent();
            DataContextChanged += DataItemUI_DataContextChanged;
        }

        Func<object> Getter;
        dynamic Setter;
        class SetterType: DynamicObject {
            Action<object> setter;
            public SetterType(Action<object> setter) => this.setter = setter;

            public override bool TryInvoke(InvokeBinder binder, object[] args, out object result) {
                setter(args[0]);
                result = null;
                return true;
            }
        }
        private void DataItemUI_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            var binding = BindingOperations.GetBinding(this, DataContextProperty);
            if (binding == null) return;
            var parentDC = Parent.GetValue(FrameworkElement.DataContextProperty);
            var type = parentDC.GetType();
            var property = binding.Path.Path;
            var pi = type.GetProperty(property);
            var attr = pi.GetCustomAttribute<DataItemAttribute>();

            Getter = () => pi.GetValue(parentDC);

            if (attr != null) {
                if (attr.Title == null) Title = property;
                else Title = attr.Title;
                MU = attr.MU;
                Setter = null;
                if (attr.Edit || CustomSetter != null) {
                    if (CustomSetter == null) {
                        Setter = new SetterType(
                            (value) => {
                                value = Convert.ChangeType(value, pi.PropertyType);
                                pi.SetValue(parentDC, value);
                            });
                    }
                    else Setter = CustomSetter;
                }
            }
        }

        void UserControl_MouseDown(object sender, MouseButtonEventArgs e) => Edit();

        void Edit() {
            if (Setter == null) return;
            var pad = SourceCollection == null ? (Window)new Keypad(this) : (Window)new SelectPad(this); 
            if (pad.ShowDialog().Value) Setter(pad.Tag);
        }

        public string FormattedValue {
            get {
                return Getter().ToString();
            }
        }

        #region Dependency Properties
        public string Title {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(DataItemUI));

        public string MU {
            get { return (string)GetValue(MUProperty); }
            set { SetValue(MUProperty, value); }
        }

        public static readonly DependencyProperty MUProperty = DependencyProperty.Register("MU", typeof(string), typeof(DataItemUI));

        public ICollection SourceCollection {
            get { return (ICollection)GetValue(SourceCollectionProperty); }
            set { SetValue(SourceCollectionProperty, value); }
        }

        public static readonly DependencyProperty SourceCollectionProperty =
            DependencyProperty.Register("SourceCollection", typeof(ICollection), typeof(DataItemUI));

        public DynamicObject CustomSetter {
            get { return (DynamicObject)GetValue(CustomSetterProperty); }
            set { SetValue(CustomSetterProperty, value); }
        }

        public static readonly DependencyProperty CustomSetterProperty =
            DependencyProperty.Register("CustomSetter", typeof(DynamicObject), typeof(DataItemUI));

        public string FieldName {
            get { return (string)GetValue(FieldNameProperty); }
            set { SetValue(FieldNameProperty, value); }
        }

        public static readonly DependencyProperty FieldNameProperty =
            DependencyProperty.Register("FieldName", typeof(string), typeof(DataItemUI));
        #endregion

    }
}

