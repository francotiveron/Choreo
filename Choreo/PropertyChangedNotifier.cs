using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Choreo
{
    public class PropertyChangedNotifier: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public delegate void NotifyContinuation(params string[] props);
        protected NotifyContinuation Notify([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            var statusProp = $"{name}Status";
            if (GetType().GetProperty(statusProp) is PropertyInfo pi)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(statusProp));
            return NotifyContinuator;
        }
        void NotifyContinuator(params string[] props) {
            foreach(var prop in props) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
