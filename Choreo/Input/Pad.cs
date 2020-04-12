using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Choreo.Input {
    public abstract class Pad : Window, INotifyPropertyChanged {
        #region static members
        public static Pad From(object o) {
            BindingExpression bx = null;
            Window owner = null;

            switch (o) {
                case Label label:
                    bx = label.GetBindingExpression(ContentProperty);
                    owner = GetWindow(label);
                    break;
                case DataItemUI diui:
                    bx = diui.GetBindingExpression(DataContextProperty);
                    owner = GetWindow(diui);
                    break;
            }

            if (bx != null) {
                var res = new NumericPad();
                res.bx = bx;
                res.Owner = owner;
                res.Loaded += Pad_Loaded;
                return res;
            }

            return null;
        }

        private static void Pad_Loaded(object sender, RoutedEventArgs e) => (sender as Pad).Init();

        public static string Edit(object o) {
            if (From(o) is Pad pad && pad.ShowDialog().Value) return pad.Result;
            return null;
        }
        #endregion

        BindingExpression bx = null;
        object obj = null;
        Type type;
        PropertyInfo pi = null;
        protected Pad() => DataContext = this;

        void Init() {
            obj = bx.ResolvedSource;
            type = obj.GetType();
            pi = type.GetProperty(bx.ResolvedSourcePropertyName);
            Result = pi.GetValue(obj).ToString();
        }

        string result;
        public string Result {
            get { return result; }
            set { result = value; OnPropertyChanged("Result"); }
        }

        protected void OnPadEvent(string e) {
            switch (e) {
                case "ESC":
                    DialogResult = false;
                    break;

                case "RETURN":
                    var newVal = Convert.ChangeType(Result, pi.PropertyType);
                    pi.SetValue(obj, newVal);
                    DialogResult = true;
                    break;

                case "BACK":
                    if (Result.Length > 0)
                        Result = Result.Remove(Result.Length - 1);
                    break;

                default:
                    Result += e;
                    break;
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string info) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion
    }
}
