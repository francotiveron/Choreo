using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using static Choreo.Globals;

namespace Choreo
{
    public class Motion: PropertyChangedNotifier {
        public Motion() { }

        Axis hook;
        public Axis Hook {
            get => hook; 
            set {
                hook = value;
                for (int i = 0; i < Motors.Length; i++) Motors[i] = false;
                for (int i = 0; i < Groups.Length; i++) Groups[i] = false;

                if (hook is Motor m) Motors[m.Index] = true;
                else
                if (hook is Group g) Groups[g.Index] = true;

                Velocity = hook.DefVel;
                Acceleration = hook.DefAcc;
                Deceleration = hook.DefDec;
            }
        }

        public bool? Rotational { get { return Hook.RotationalAxis; } }

        public bool[] Motors { get; } = new bool[16];
        public bool[] Groups { get; } = new bool[8];

        public bool Contains(Axis ax) => (ax is Motor ? Motors : Groups)[ax.Index];

        public IEnumerable<Axis> Axes => VM.Motors.Where(m => Motors[m.Index]).Cast<Axis>().Concat(VM.Groups.Where(g => Groups[g.Index]).Cast<Axis>());

        public double MinVel => Axes.Select(ax => ax.MinVel).Max();
        public double MaxVel => Axes.Select(ax => ax.MaxVel).Min();
        public double MinAcc => Axes.Select(ax => ax.MinAcc).Max();
        public double MaxAcc => Axes.Select(ax => ax.MaxAcc).Min();
        public double MinDec => Axes.Select(ax => ax.MinDec).Max();
        public double MaxDec => Axes.Select(ax => ax.MaxDec).Min();

        private bool relative;
        public bool Relative {
            get { return relative; }
            set { relative = value; Notify(); }
        }

        double relativeSetpoint;
        [DataItem(title:"Relative Setpoint")]
        public double RelativeSetpoint {
            get { return relativeSetpoint; }
            set { relativeSetpoint = value; Notify(); }
        }
        public Status RelativeSetpointStatus => Status.Ok;

        double absoluteSetpoint;
        [DataItem(title: "Absolute Setpoint")]
        public double AbsoluteSetpoint {
            get { return absoluteSetpoint; }
            set { absoluteSetpoint = value; Notify(); }
        }
        public Status AbsoluteSetpointStatus => Status.Ok;

        double velocity;
        [DataItem]
        public double Velocity {
            get { return velocity; }
            set { velocity = value; Notify(); }
        }
        public Status VelocityStatus => Velocity < MinVel || Velocity > MaxVel;

        double acceleration;
        [DataItem]
        public double Acceleration {
            get { return acceleration; }
            set { acceleration = value; Notify(); }
        }
        public Status AccelerationStatus => Acceleration < MinAcc || Acceleration > MaxAcc;

        double deceleration;
        [DataItem]
        public double Deceleration {
            get { return deceleration; }
            set { deceleration = value; Notify(); }
        }
        public Status DecelerationStatus => Deceleration < MinDec || Deceleration > MaxDec;

        public void Validate() => MultiNotify(nameof(VelocityStatus), nameof(AccelerationStatus), nameof(DecelerationStatus));
    }
}
