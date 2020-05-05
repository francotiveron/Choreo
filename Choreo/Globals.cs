using Choreo.Input;
using Choreo.Logging;
using Choreo.TwinCAT;
using QuickConverter;
using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Choreo
{
    public static class Globals {
        public static ChoreoLogger Log { get; private set; }
        public static ViewModel VM { get; private set; }
        public static IPlc Plc { get; private set; }
        static Globals() {
            Log = ChoreoLogger.GetLogging();
            VM = new ViewModel();
            Storage.LoadAll();
            Plc = PlcFactory.New();
            Plc.SymbolsUpdated += Plc_SymbolsUpdated;
        }

        private static void Plc_SymbolsUpdated(object sender, EventArgs e) => VM.Init();
        public struct Status {
            public enum Values { Ok, Warning, Error };
            public static Values Ok => Values.Ok;
            public static Values Warning => Values.Warning;
            public static Values Error => Values.Error;
            public Status(Values value) => Value = value;
            public Values Value { get; set; }
            public static implicit operator Values(Status status) => status.Value;
            public static implicit operator Status(Values value) => new Status(value);
            public static implicit operator Status(bool error) => error ? Error : Ok;
            public static implicit operator int(Status status) => (int)status.Value;
            public override string ToString() => Value.ToString();
        }

        public static object FindWPFTreeUp(this DependencyObject start, Func<DependencyObject, bool> selector, Func<DependencyObject, object> mapper) {
            for (DependencyObject depo = start; depo != null; depo = GetParentObject(depo)) if (selector(depo)) return mapper(depo);
            return null;
        }
        public static DependencyObject FindWPFTreeUp(this DependencyObject start, Func<DependencyObject, bool> selector) {
            return (DependencyObject)FindWPFTreeUp(start, selector, (o) => o);
        }

        static DependencyObject GetParentObject(DependencyObject child) {
            if (child == null) return null;

            ContentElement contentElement = child as ContentElement;
            if (contentElement != null) {
                DependencyObject parent = ContentOperations.GetParent(contentElement);
                if (parent != null) return parent;

                FrameworkContentElement fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            FrameworkElement frameworkElement = child as FrameworkElement;
            if (frameworkElement != null) {
                DependencyObject parent = frameworkElement.Parent;
                if (parent != null) return parent;
            }

            return VisualTreeHelper.GetParent(child);
        }

        public static bool DataItemsValid(UIElementCollection children) {
            foreach (var uie in children) {
                switch (uie) {
                    case DataItemUI diui:
                        if (diui.Status == Status.Error) return false;
                        break;
                    case Panel panel:
                        if (!DataItemsValid(panel.Children)) return false;
                        break;
                }
            }
            return true;
        }
    }
}
