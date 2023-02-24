using Choreo.TwinCAT;
using System.Linq;
using System.Windows.Media;
using static Choreo.Globals;

namespace Choreo
{
    public partial class Group: Axis {
        public static readonly Color[] GroupColors = { Colors.Red, Colors.Blue, Colors.Purple, Colors.Orange, Colors.DarkCyan, Colors.Green, Colors.Brown, Colors.Magenta };
        public Group(int index) : base(index) { }

        bool save;
        [Plc("GVL.Group_NN_Save")]
        public bool Save
        {
            get => save;
            set { save = !value; Notify()(nameof(Save)); }
        }

        double toleranceRotations;
        [Plc("Tolerance_Value")]
        public double ToleranceRotations
        {
            get => toleranceRotations;
            set { toleranceRotations = value; Notify(nameof(ToleranceValue)); }
        }

        [DataItem(title: "Group Tolerance")]
        public double ToleranceValue
        {
            get => ToleranceRotations / RotationsPerEU;
            set { ToleranceRotations = value * RotationsPerEU; Notify(); }
        }

        public override Color Color {
            get {
                if (Active) return Colors.Lime;
                return GroupColors[Index];
            }
        }
        public bool IsPreset => VM.Presets.Any(p => p.ContainsGroup(Index));

        public override Status MinVelStatus {
            get
            {
                if (VM.Motors.Any(m => m.Group == Number && m.MinVel > MinVel))
                    return Status.Error;
                else
                    return base.MinVelStatus;
            }
        }

        public override Status MaxVelStatus
        {
            get
            {
                if (VM.Motors.Any(m => m.Group == Number && m.MaxVel < MaxVel))
                    return Status.Error;
                else
                    return base.MaxVelStatus;
            }
        }
        public override Status MaxAccStatus
        {
            get
            {
                if (VM.Motors.Any(m => m.Group == Number && m.MaxAcc < MaxAcc))
                    return Status.Error;
                else
                    return base.MaxAccStatus;
            }
        }
        public override Status MaxDecStatus
        {
            get
            {
                if (VM.Motors.Any(m => m.Group == Number && m.MaxDec < MaxDec))
                    return Status.Error;
                else
                    return base.MaxDecStatus;
            }
        }
    }
}
