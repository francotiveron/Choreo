﻿using Choreo.Logging;
using Choreo.TwinCAT;
using System;
using System.Collections.Generic;
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
            VM.Init();
        }

        public struct Status {
            public enum Values { Ok, Warning, Alert, Error };
            public static Values Ok => Values.Ok;
            public static Values Warning => Values.Warning;
            public static Values Alert => Values.Alert;
            public static Values Error => Values.Error;
            public Values Value { get; set; }
            public Status(Values value) => Value = value;

            public Status(ushort faultCode)
            {
                switch (faultCode & 0xC000)
                {
                    case 0x0000: Value = Ok; break;
                    case 0x4000: Value = Warning; break;
                    case 0x8000: Value = Alert; break;
                    case 0xC000: Value = Error; break;
                    default: throw new ArgumentOutOfRangeException(nameof(faultCode));
                }
            }

            public static implicit operator Values(Status status) => status.Value;
            public static implicit operator Status(Values value) => new Status(value);
            public static implicit operator Status(bool error) => error ? Error : Ok;
            public static implicit operator bool(Status status) => status == Error;
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

        public static bool AreCompatible(Motor m, Group g) => 
            g.MaxVel <= m.MaxVel 
            && g.MinVel >= m.MinVel 
            && g.MaxAcc <= m.MaxAcc 
            && g.MaxDec <= m.MaxDec;

        public static object Default(this Type t) {
            if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                return Activator.CreateInstance(t);
            else
                return null;
        }

        public static Dictionary<ushort, string> FaultCodeMap;

    }
}
