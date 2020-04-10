using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Choreo {
    public class Motion: PropertyChangedNotifier {
        public Motion() { }

        public object Hook { get; set; }

        public bool[] Motors { get; } = new bool[16];
        public bool[] Groups { get; } = new bool[8];

        private bool relative;
        public bool Relative {
            get { return relative; }
            set { relative = value; OnPropertyChanged(); }
        }

        double relativeSetpoint;
        [DataItem(edit: true)]
        public double RelativeSetpoint {
            get { return relativeSetpoint; }
            set { relativeSetpoint = value; OnPropertyChanged(); }
        }

        double absoluteSetpoint;
        [DataItem(edit: true)]
        public double AbsoluteSetpoint {
            get { return absoluteSetpoint; }
            set { absoluteSetpoint = value; OnPropertyChanged(); }
        }

        double velocity;
        [DataItem(edit: true)]
        public double Velocity {
            get { return velocity; }
            set { velocity = value; OnPropertyChanged(); }
        }

        double acceleration;
        [DataItem(edit: true)]
        public double Acceleration {
            get { return acceleration; }
            set { acceleration = value; OnPropertyChanged(); }
        }

        double deceleration;
        [DataItem(edit: true)]
        public double Deceleration {
            get { return deceleration; }
            set { deceleration = value; OnPropertyChanged(); }
        }
    }
}
