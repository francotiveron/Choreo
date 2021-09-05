using System;
using TwinCAT.TypeSystem;

namespace Choreo.TwinCAT
{
    public interface ITag {
        ISymbol Symbol { get; set; }
        void Push(object value);
    }
    
    public class Tag: ITag {
        public Tag(Action<object> pusher = null) { 
            Pusher = pusher;
        }
        public ISymbol Symbol { get; set; }
        public Action<object> Pusher { get; private set; }

        public virtual void Push(object value) {
            Pusher?.Invoke(value);
        }

        public static string GetKey(string typeName, string propName, int? number = null) {
            if (number.HasValue) return $"{typeName}_{number:00}_{propName}";
            else return $"{typeName}_{propName}";
        }
        public static string GetKey(object obj, string propName) {
            var type = obj.GetType();
            var numberPi = type.GetProperty("Number");
            int? number = (int?)numberPi?.GetValue(obj);
            return GetKey(type.Name, propName, number);
        }
    }
}
