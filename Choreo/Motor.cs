using Choreo.TwinCAT;
using System.Linq;
using System.Windows.Media;
using static Choreo.Globals;

namespace Choreo {
    public class Motor : Axis {
        public Motor(int index) : base(index) { }

        #region Runtime+PLC Properties
        bool fwdLim;
        [Plc("Fwd_Lim")]
        public bool FwdLim {
            get => fwdLim;
            set { fwdLim = value; OnPropertyChanged(); }
        }

        bool revLim;
        [Plc("Rev_Lim")]
        public bool RevLim {
            get => revLim;
            set { revLim = value; OnPropertyChanged(); }
        }

        bool fwdUltLim;
        [Plc("Fwd_Ult_Lim")]
        public bool FwdUltLim {
            get => fwdUltLim;
            set { fwdUltLim = value; OnPropertyChanged(); }
        }

        bool revUltLim;
        [Plc("Rev_Ult_Lim")]
        public bool RevUltLim {
            get => revUltLim;
            set { revUltLim = value; OnPropertyChanged(); }
        }

        bool driveStatus;
        [Plc("Drive_Status")]
        public bool DriveStatus {
            get => driveStatus;
            set { driveStatus = value; OnPropertyChanged(); }
        }

        bool eSStatus;
        [Plc("ES_Status")]
        public bool ESStatus {
            get => eSStatus;
            set { eSStatus = value; OnPropertyChanged(); }
        }
        #endregion

        #region UI Properties
        int group;
        [DataItem(title: "Axis Group")]
        public int Group {
            get => group;
            set {
                group = value;
                OnPropertyChanged(); OnPropertyChanged("Color");
            }
        }
        public bool IsGrouped => Group > 0;

        public override Color Color {
            get {
                if (Active) return Colors.Lime;
                if (Group == 0) return Colors.DarkGray;
                return Choreo.Group.GroupColors[Group - 1];
            }
        }
        public bool IsPreset => VM.Presets.Any(p => p.ContainsMotor(Index));
        #endregion
    }
}
