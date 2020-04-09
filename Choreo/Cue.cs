using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Choreo {
    public class CueRow: PropertyChangedNotifier {
        public List<int> Motors { get; private set; }
        public List<int> Groups { get; private set; }

        double target;
        [DataItem]
        public double Target {
            get { return target; }
            set { target = value; OnPropertyChanged(); }
        }

        double velocity;
        public double Velocity {
            get { return velocity; }
            set { velocity = value; OnPropertyChanged(); }
        }

        double acceleration;
        public double Acceleration {
            get { return acceleration; }
            set { acceleration = value; OnPropertyChanged(); }
        }

        double deceleration;
        public double Deceleration {
            get { return deceleration; }
            set { deceleration = value; OnPropertyChanged(); }
        }
    }

    public class Cue: PropertyChangedNotifier {
        public Cue(int index) => Index = index;

        string name;
        [DataItem(title: "Cue Name")]
        public string Name {
            get {
                if (name == null) return $"Cue {Index + 1}";
                return name;
            }
            set { name = value; OnPropertyChanged(); }
        }

        int index;
        public int Index {
            get { return index; }
            set { index = value; OnPropertyChanged(); }
        }

        private bool show;

        public bool Show {
            get { return show; }
            set { show = value; }
        }

        private TimeSpan duration;

        public TimeSpan Duration {
            get { return duration; }
            set { duration = value; }
        }

        public ObservableCollection<CueRow> Rows { get; private set; } = new ObservableCollection<CueRow>();
    }
}
