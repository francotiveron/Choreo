using System;
using System.Collections;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using static Choreo.Globals;

namespace Choreo {
    public partial class DataItemUI {
        public DataItemUI() {
            InitializeComponent();
            DataContextChanged += DataItemUI_DataContextChanged;
        }

        object dc;
        PropertyInfo pi;
        DependencyObject focusScope = null;
        private void DataItemUI_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            var binding = BindingOperations.GetBinding(this, DataContextProperty);
            if (binding == null) return;

            dc = (Parent ?? TemplatedParent).GetValue(DataContextProperty);
            var type = dc.GetType();
            var property = binding.Path.Path;
            pi = type.GetProperty(property);
            var attr = pi.GetCustomAttribute<DataItemAttribute>();

            var x = StatusCoverRectangle.GetBindingExpression(Shape.FillProperty);
            binding = new Binding($"{property}Status");
            binding.Source = dc;
            binding.Converter = x.ParentBinding.Converter;
            binding.ConverterParameter = StatusCoverRectangle;
            StatusCoverRectangle.SetBinding(Shape.FillProperty, binding);

            x = StatusBottomLine.GetBindingExpression(Shape.FillProperty);
            binding = new Binding($"{property}Status");
            binding.Source = dc;
            binding.Converter = x.ParentBinding.Converter;
            binding.ConverterParameter = StatusBottomLine;
            StatusBottomLine.SetBinding(Shape.FillProperty, binding);

            if (attr != null) {
                Title = attr.Title ?? property;
                MU = attr.MU;
            }

            focusScope = this.FindWPFTreeUp((depo) => depo.GetValue(FocusManager.IsFocusScopeProperty) is bool b && b);
        }

        void Set(object v) => pi.SetValue(dc, v);

        void Set(string v) {
            object value = Convert.ChangeType(v, pi.PropertyType);
            Set(value);
        }
        public string StrVal {
            get => Value.Content.ToString();
            set => Set(value);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            if (ValueFontSize != null) Value.SetValue(FontSizeProperty, ValueFontSize);
            if (LabelFontSize != null) {
                Label.SetValue(FontSizeProperty, LabelFontSize);
                Unit.SetValue(FontSizeProperty, LabelFontSize);
            }
        }

        static public object ValueFontSize { get; set; }
        static public object LabelFontSize { get; set; }
        void UserControl_MouseDown(object sender, MouseButtonEventArgs e) {
            if (Focusable && focusScope != null) FocusManager.SetFocusedElement(focusScope, this);
        }

        public DataItemUI Navigate(string toWhere) {
            DataItemUI diui = null;

            switch (toWhere) {
                case "NEXT":
                    diui = Next;
                    break;
                case "PREV":
                    diui = Prev;
                    break;
            }
            return diui;
        }

        public DataItemUI Next {
            get {
                var diui = this;
                for (diui = EditOrderNext; diui != this && !diui.IsEnabled; diui = diui.EditOrderNext) ;
                return diui;
            }
        }

        public DataItemUI Prev {
            get {
                var diui = this;
                for (diui = EditOrderPrev; diui != this && !diui.IsEnabled; diui = diui.EditOrderPrev) ;
                return diui;
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

        //public ICollection SourceCollection {
        //    get { return (ICollection)GetValue(SourceCollectionProperty); }
        //    set { SetValue(SourceCollectionProperty, value); }
        //}

        //public static readonly DependencyProperty SourceCollectionProperty =
        //    DependencyProperty.Register("SourceCollection", typeof(ICollection), typeof(DataItemUI));

        //public string FieldName {
        //    get { return (string)GetValue(FieldNameProperty); }
        //    set { SetValue(FieldNameProperty, value); }
        //}

        //public static readonly DependencyProperty FieldNameProperty =
        //    DependencyProperty.Register("FieldName", typeof(string), typeof(DataItemUI));

        public DataItemUI EditOrderNext {
            get { return (DataItemUI)GetValue(EditOrderNextProperty); }
            set { SetValue(EditOrderNextProperty, value); }
        }

        public static readonly DependencyProperty EditOrderNextProperty =
            DependencyProperty.Register("EditOrderNext", typeof(DataItemUI), typeof(DataItemUI));

        public DataItemUI EditOrderPrev {
            get { return (DataItemUI)GetValue(EditOrderPrevProperty); }
            set { SetValue(EditOrderPrevProperty, value); }
        }

        public static readonly DependencyProperty EditOrderPrevProperty =
            DependencyProperty.Register("EditOrderPrev", typeof(DataItemUI), typeof(DataItemUI));
        #endregion
    }

    public class StatusBrushConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null) return null;
            DataStates status = (DataStates)value;
            var color = Colors.Gray;
            var opacity = 0.0;

            switch(status) {
                case DataStates.Warning:
                    color = Colors.Yellow;
                    opacity = 0.4;
                    break;
                case DataStates.Error:
                    color = Colors.Red;
                    opacity = 0.4;
                    break;
            }
            var brush = new SolidColorBrush(color);
            if (parameter is Rectangle r && r.Name == "StatusCoverRectangle") brush.Opacity = opacity;

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }


    public class DataItemEnablingConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (Visibility)value == Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

    [System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    sealed class DataItemAttribute : Attribute {
        public DataItemAttribute(string mu = null, string title = null) {
            Title = title;
            MU = mu;
        }

        public string Title { get; private set; }
        public string MU { get; private set; }
    }
}

