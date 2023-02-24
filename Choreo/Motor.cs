using Choreo.TwinCAT;
using System.Linq;
using System.Windows.Media;
using static Choreo.Globals;

namespace Choreo
{
    public class Motor : Axis {
        public Motor(int index) : base(index) { }

        //public override Status AxisStatus => base.AxisStatus || DriveStatus || ESStatus;
        public override Status AxisStatus
        {
            get
            {
                if (DriveStatus || ESStatus)
                    return Status.Error;
                else
                    return base.AxisStatus;

            }
        }
        public override string AxisStatusDescription
        {
            get {
                if (ESStatus) return "E-Stop";
                if (DriveStatus) return "Drive Fault";
                return base.AxisStatusDescription;
            }
        }

        bool followErrorEnable;
        [Plc("Follow_Error_Enable", false)]
        public bool FollowErrorEnable
        {
            get => followErrorEnable;
            set { followErrorEnable = value; Notify(); }
        }

        [Plc]
        public override bool Present {
            get => base.Present;
            protected set { base.Present = value; Notify(); }
        }

        bool fwdLim;
        [Plc("Fwd_Lim", false)]
        public bool FwdLim {
            get => fwdLim;
            set { fwdLim = value; Notify(); }
        }

        bool revLim;
        [Plc("Rev_Lim", false)]
        public bool RevLim {
            get => revLim;
            set { revLim = value; Notify(); }
        }

        bool fwdUltLim;
        [Plc("Fwd_Ult_Lim", false)]
        public bool FwdUltLim {
            get => fwdUltLim;
            set { fwdUltLim = value; Notify(); }
        }

        bool revUltLim;
        [Plc("Rev_Ult_Lim", false)]
        public bool RevUltLim {
            get => revUltLim;
            set { revUltLim = value; Notify(); }
        }

        bool driveStatus;
        [Plc("Drive_Status")]
        public bool DriveStatus {
            get => driveStatus;
            set { driveStatus = value; Notify()(nameof(AxisStatus)); }
        }

        bool eSStatus;
        [Plc("ES_Status")]
        public bool ESStatus {
            get => eSStatus;
            set { eSStatus = !value; Notify()(nameof(AxisStatus)); }
        }

        [Plc("Rotations_Per_EU", false)]
        public override double RotationsPerEU
        {
            get => base.RotationsPerEU;

            set
            {
                var followingError = FollowingError;
                base.RotationsPerEU = value;
                FollowingError = followingError;
                Notify()(nameof(FollowingError));
            }
        }

        double pGain;
        [DataItem("r/s/r", "P-Gain"), Plc("PGain", false)]
        public double PGain
        {
            get => pGain;
            set { pGain = value; Notify(); }
        }
        public Status PGainStatus => !(PGain >= 0.0001 && PGain <= 10000);

        double jerk;
        [DataItem("r/s2", "Jerk"), Plc("Jerk", false)]
        public double Jerk
        {
            get => jerk;
            set { jerk = value; Notify(); }
        }
        public Status JerkStatus => !(Jerk >= 0 && Jerk <= 10000);

        double refVel;
        [DataItem("r/s", "Ref.Velocity"), Plc("Ref_Vel", false)]
        public double RefVel
        {
            get => refVel;
            set { refVel = value; Notify(); }
        }
        public Status RefVelStatus => !(RefVel >= 0 && RefVel <= 10000);


        double followingErrorRotations;
        [Plc("Following_Error", false)]
        public double FollowingErrorRotations
        {
            get => followingErrorRotations;
            set { followingErrorRotations = value; Notify()(nameof(FollowingError)); }
        }

        [DataItem(title: "Following Error")]
        public double FollowingError
        {
            get => followingErrorRotations / RotationsPerEU;
            set { followingErrorRotations = value * RotationsPerEU; Notify()(nameof(FollowingErrorStatus)); }
        }
        public Status FollowingErrorStatus => Status.Ok;

        public override bool IsGrouped => Group > 0;

        public override Color Color {
            get {
                if (Active) return Colors.Lime;
                if (Group == 0) return Colors.DarkGray;
                return Choreo.Group.GroupColors[Group - 1];
            }
        }
        public bool IsPreset => VM.Presets.Any(p => p.ContainsMotor(Index));
    }
}
