using Choreo.Input;
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
using TwinCAT.Ads;
using TwinCAT.TypeSystem;
using static Choreo.Globals;

namespace Choreo {
    public partial class DataItemUI : IStrVal {
        public DataItemUI() {
            InitializeComponent();
            DataContextChanged += DataItemUI_DataContextChanged;
        }

        object dc;
        PropertyInfo pi, statusPi;
        DependencyObject focusScope = null;
        public bool IsPosition { get; private set; }

        private void DataItemUI_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            var binding = BindingOperations.GetBinding(this, DataContextProperty);
            if (binding == null) return;

            IsPosition = binding.Converter == Application.Current.Resources["PositionConverter"];

            dc = (Parent ?? TemplatedParent).GetValue(DataContextProperty);
            var type = dc.GetType();
            var property = binding.Path.Path;
            pi = type.GetProperty(property);
            statusPi = type.GetProperty($"{property}Status");
            var attr = pi.GetCustomAttribute<DataItemAttribute>();

            var x = StatusCoverRectangle.GetBindingExpression(Shape.FillProperty);
            binding = new Binding($"{property}Status");
            binding.Source = dc;
            binding.Converter = x.ParentBinding.Converter;
            binding.ConverterParameter = StatusCoverRectangle.Name;
            StatusCoverRectangle.SetBinding(Shape.FillProperty, binding);

            x = StatusBottomLine.GetBindingExpression(Shape.FillProperty);
            binding = new Binding($"{property}Status");
            binding.Source = dc;
            binding.Converter = x.ParentBinding.Converter;
            binding.ConverterParameter = StatusBottomLine.Name;
            StatusBottomLine.SetBinding(Shape.FillProperty, binding);

            if (attr != null) {
                Title = attr.Title ?? property;
                MU = attr.MU;
            }

            focusScope = this.FindWPFTreeUp((depo) => depo.GetValue(FocusManager.IsFocusScopeProperty) is bool b && b);
        }

        void Set(object v) => pi.SetValue(dc, v);

        void Set(string v) {
            object value = null;
            if (IsPosition) {
                if (FeetInchesConvert.TryParse(v, out double? feet)) value = feet;
            }
            else {
                value = Convert.ChangeType(v, pi.PropertyType);
                if (value is double dv) value = Math.Round(dv, 3);
            }

            Set(value);
        }
        public string StrVal {
            get => Value.Content.ToString();
            set => Set(value);
        }

        public bool StandAlone { get; set; }

        public Status? Status => (Status?)statusPi?.GetValue(dc);

        void UserControl_Loaded(object sender, RoutedEventArgs e) {
            if (ValueFontSize != null) Value.SetValue(FontSizeProperty, ValueFontSize);
            if (LabelFontSize != null) {
                Label.SetValue(FontSizeProperty, LabelFontSize);
                Unit.SetValue(FontSizeProperty, LabelFontSize);
            }
            UpdateBackground();
        }

        void UpdateBackground() {
            Brush brush = null;

            if (IsFocused) brush = (Brush)Application.Current.Resources["DataItemFocusedBackground"];
            else if (Focusable || StandAlone) brush = (Brush)Application.Current.Resources["DataItemUnfocusedBackground"];

            Background = brush;
        }
        protected override void OnGotFocus(RoutedEventArgs e) {
            base.OnGotFocus(e);
            UpdateBackground();
        }

        protected override void OnLostFocus(RoutedEventArgs e) {
            base.OnLostFocus(e);
            UpdateBackground();
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
                var diui = EditOrderNext;
                for (; diui != null && diui != this && !diui.IsEnabled; diui = diui.EditOrderNext) ;
                return diui;
            }
        }

        public DataItemUI Prev {
            get {
                var diui = EditOrderPrev;
                for (; diui != null && diui != this && !diui.IsEnabled; diui = diui.EditOrderPrev) ;
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

    public class DataItemStatusBrushConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null || !(value is Status)) return null;
            Status status = (Status)value;
            var color = Colors.Transparent;
            var opacity = 0.0;

            switch(status.Value) {
                case Status.Values.Warning:
                    color = Colors.Yellow;
                    opacity = 0.4;
                    break;
                case Status.Values.Error:
                    color = Colors.Red;
                    opacity = 0.4;
                    break;
            }

            if (opacity == 0.0) return null;

            var brush = new SolidColorBrush(color);
            if ((string)parameter == "StatusCoverRectangle") brush.Opacity = opacity;

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

    public class PositionConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var feet = (double)value;
            return FeetInchesConvert.ToString(feet);
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

