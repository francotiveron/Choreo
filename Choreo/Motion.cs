using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Choreo.Globals;

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
        [DataItem(title:"Relative Setpoint")]
        public double RelativeSetpoint {
            get { return relativeSetpoint; }
            set { relativeSetpoint = value; OnPropertyChanged(); if (value > 2.0) RelativeSetpointStatus = DataStates.Warning; else RelativeSetpointStatus = DataStates.OK; }
        }
        private DataStates relativeSetpointStatus;
        public DataStates RelativeSetpointStatus {
            get { return relativeSetpointStatus; }
            set { relativeSetpointStatus = value; OnPropertyChanged(); }
        }

        double absoluteSetpoint;
        [DataItem(title: "Absolute Setpoint")]
        public double AbsoluteSetpoint {
            get { return absoluteSetpoint; }
            set { absoluteSetpoint = value; OnPropertyChanged(); }
        }

        double velocity;
        [DataItem]
        public double Velocity {
            get { return velocity; }
            set { velocity = value; OnPropertyChanged(); }
        }

        double acceleration;
        [DataItem]
        public double Acceleration {
            get { return acceleration; }
            set { acceleration = value; OnPropertyChanged(); }
        }

        double deceleration;
        [DataItem]
        public double Deceleration {
            get { return deceleration; }
            set { deceleration = value; OnPropertyChanged(); }
        }

        public void Upload() {

        }
    }
}
