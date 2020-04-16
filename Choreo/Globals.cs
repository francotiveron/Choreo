using Choreo.Input;
using System.Windows;
using System.Windows.Controls;

namespace Choreo
{
    public static class Globals {
        public static ViewModel VM { get; } = new ViewModel();
        static Globals() => Storage.LoadAll();
        public static void Edit(object o) => Pad.Edit(o);
        public enum DataStates { OK, Warning, Error };
    }
}
