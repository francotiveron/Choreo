using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Choreo
{
    public class PropertyChangedNotifier: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public delegate void MultiNotifyDelegate(params string[] props);
        protected MultiNotifyDelegate Notify([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            var statusProp = $"{name}Status";
            if (GetType().GetProperty(statusProp) is PropertyInfo pi)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(statusProp));
            return MultiNotify;
        }
        protected void MultiNotify(params string[] props) {
            foreach(var prop in props) PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
