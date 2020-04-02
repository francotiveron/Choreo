using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Choreo
{
    public class Motor: PropertyChangedNotifier
    {
        public Motor(int index) { Index = index; }
        public float Position => 3.5F;

        private bool isOK;
        public bool IsOK {
            get => isOK; 
            set { isOK = value; OnPropertyChanged(); }
        }

        string name;
        public string Name {
            get {
                if (name == null) return $"Motor {Index + 1}";
                return name;
            }
            set { name = value; OnPropertyChanged(); }
        }

        public int Index { get; set; }

        int group;
        public int Group {
            get => group;
            set { 
                group = value; 
                OnPropertyChanged(); OnPropertyChanged("Color"); 
            }
        }

        public Color Color {
            get {
                if (group == 0) return Colors.Gray;
                return Choreo.Group.GroupColors[Group - 1];
            }
        }

    }
}
