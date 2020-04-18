using Choreo.Input;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Choreo
{
    public static class Globals {
        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static ViewModel VM { get; } = new ViewModel();
        static Globals() => Storage.LoadAll();
        public static void Edit(object o) => Pad.Edit(o);
        public enum DataStates { OK, Warning, Error };
        public static object FindWPFTreeUp(this DependencyObject start, Func<DependencyObject, bool> selector, Func<DependencyObject, object> mapper) {
            for (DependencyObject depo = start; depo != null; depo = VisualTreeHelper.GetParent(depo)) if (selector(depo)) return mapper(depo);
            return null;
        }
        public static DependencyObject FindWPFTreeUp(this DependencyObject start, Func<DependencyObject, bool> selector) {
            return (DependencyObject)FindWPFTreeUp(start, selector, (o) => o);
        }
    }
}
