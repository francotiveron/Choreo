using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Choreo
{
    public class Motor: PropertyChangedNotifier
    {
        public float Position => 3.5F;

        private bool isOK;
        public bool IsOK {
            get => isOK; 
            set {
                isOK = value;
                OnPropertyChanged();
            }
        }
    }
}
