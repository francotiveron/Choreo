using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Choreo
{
    static class Globals {
        public static ViewModel VM {
            get {
                return (ViewModel)Application.Current.FindResource("ChoreoViewModel");
            }
        }
    }
}
