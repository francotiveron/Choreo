using Choreo.TwinCAT;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static Choreo.Globals;

namespace Choreo {

    public class Motor : PropertyChangedNotifier {
        public Motor(int index) { Index = index; }

        public double Rotations {
            get => Position / FeetPerRotation;
            set => Position = value * FeetPerRotation;
        }

        #region Runtime+PLC Properties
        double position;
        [DataItem, Plc("Act_Pos")]
        public double Position {
            get => position;
            private set { position = value; OnPropertyChanged(); }
        }
        private int positionStatus;
        public int PositionStatus {
            get { return positionStatus; }
            set { positionStatus = value; OnPropertyChanged(); }
        }

        double load;
        [DataItem("lbs"), Plc]
        public double Load {
            get => load;
            set { load = value; OnPropertyChanged(); }
        }
        private int loadStatus;
        public int LoadStatus {
            get { return loadStatus; }
            set { loadStatus = value; OnPropertyChanged(); }
        }

        [Plc("Min_Load")]
        public double LoadMin {
            set => MinLoad = value;
        }

        [Plc("Max_Load")]
        public double LoadMax {
            set => MaxLoad = value;
        }

        double moveVal;
        [Plc("Move_Val")]
        public double MoveVal {
            get => moveVal;
            set { moveVal = value; OnPropertyChanged(); }
        }

        double accel;
        [Plc]
        public double Accel {
            get => accel;
            set { accel = value; OnPropertyChanged(); }
        }

        double decel;
        [Plc]
        public double Decel {
            get => decel;
            set { decel = value; OnPropertyChanged(); }
        }

        double velocity;
        [Plc]
        public double Velocity {
            get => velocity;
            set { velocity = value; OnPropertyChanged(); }
        }

        bool mAEnable;
        [Plc("MA_Enable")]
        public bool MAEnable {
            get => mAEnable;
            set { mAEnable = value; OnPropertyChanged(); }
        }

        bool mREnable;
        [Plc("MR_Enable")]
        public bool MREnable {
            get => mREnable;
            set { mREnable = value; OnPropertyChanged(); }
        }

        bool jogUpEnable;
        [Plc("Jog_Up_Enable")]
        public bool JogUpEnable {
            get => jogUpEnable;
            set { jogUpEnable = value; OnPropertyChanged(); }
        }


        bool jogDnEnable;
        [Plc("Jog_Dn_Enable")]
        public bool JogDnEnable {
            get => jogDnEnable;
            set { jogDnEnable = value; OnPropertyChanged(); }
        }

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

        bool overLoad;
        [Plc("Over_Load")]
        public bool OverLoad {
            get => overLoad;
            set { overLoad = value; OnPropertyChanged(); }
        }

        bool underLoad;
        [Plc("Under_Load")]
        public bool UnderLoad {
            get => underLoad;
            set { underLoad = value; OnPropertyChanged(); }
        }

        bool driveStatus;
        [Plc("Drive_Status")]
        public bool DriveStatus {
            get => driveStatus;
            set { driveStatus = value; OnPropertyChanged(); }
        }

        bool moveComplete;
        [Plc("Move_Complete")]
        public bool MoveComplete {
            get => moveComplete;
            set { moveComplete = value; OnPropertyChanged(); }
        }

        bool userEnable;
        [Plc("User_Enable")]
        public bool UserEnable {
            get => userEnable;
            set { userEnable = value; OnPropertyChanged(); }
        }

        bool eSStatus;
        [Plc("ES_Status")]
        public bool ESStatus {
            get => eSStatus;
            set { eSStatus = value; OnPropertyChanged(); }
        }

        bool active;
        [Plc]
        public bool Active {
            get => active;
            set { active = value; OnPropertyChanged(); }
        }

        bool present;
        [Plc]
        public bool Present {
            get => present;
            set { present = value; OnPropertyChanged(); }
        }

        double calibrationValue;
        [Plc("Calibration_Value")]
        public double CalibrationValue {
            get => calibrationValue;
            set { calibrationValue = value; OnPropertyChanged(); }
        }

        bool calibrationSave;
        [Plc("Calibration_Save")]
        public bool CalibrationSave {
            get => calibrationSave;
            set { calibrationSave = value; OnPropertyChanged(); }
        }

        bool isOK;
        public bool IsOK {
            get => isOK;
            set { isOK = value; OnPropertyChanged(); }
        }
        #endregion

        #region UI Properties
        string name;
        [DataItem(title: "Axis Name"), Persistent]
        public string Name {
            get {
                if (name == null) return $"Motor {Index + 1:00}";
                return name;
            }
            set { name = value; OnPropertyChanged(); }
        }

        public int Index { get; set; }
        public int Number => Index + 1;

        int group;
        [DataItem(title: "Axis Group"), Persistent]
        public int Group {
            get => group;
            set {
                group = value;
                OnPropertyChanged(); OnPropertyChanged("Color");
            }
        }
        public bool IsGrouped => Group > 0;

        public Color Color {
            get {
                if (Group == 0) return Colors.DarkGray;
                return Choreo.Group.GroupColors[Group - 1];
            }
        }

        int presetTouches;
        public int PresetTouches {
            get => presetTouches;
            set {
                presetTouches = value;
                OnPropertyChanged();
            }
        }
        public void PresetTouch() => ++PresetTouches;

        public bool IsPreset => VM.Presets.Any(p => p.ContainsMotor(Index));
        #endregion

        #region Settings
        double setPosition;
        [DataItem(title: "Set Position"), Persistent]
        public double SetPosition {
            get => setPosition;
            set { setPosition = value; OnPropertyChanged(); }
        }
        private int setPositionStatus;
        public int SetPositionStatus {
            get { return setPositionStatus; }
            set { setPositionStatus = value; OnPropertyChanged(); }
        }

        double softUp;
        [DataItem(title: "Soft Up/Max Limit"), Persistent]
        public double SoftUp {
            get => softUp;
            set { softUp = value; OnPropertyChanged(); }
        }
        double softDn;
        [DataItem(title: "Soft Down/Min Limit"), Persistent]
        public double SoftDn {
            get => softDn;
            set { softDn = value; OnPropertyChanged(); }
        }
        double minAcc;
        [DataItem("fpm2", "Min Acceleration"), Persistent]
        public double MinAcc {
            get => minAcc;
            set { minAcc = value; OnPropertyChanged(); }
        }
        double maxAcc;
        [DataItem("fpm2", "Max Acceleration"), Persistent]
        public double MaxAcc {
            get => maxAcc;
            set { maxAcc = value; OnPropertyChanged(); }
        }
        double defAcc;
        [DataItem("fpm2", "Default Acceleration"), Persistent]
        public double DefAcc {
            get => defAcc;
            set { defAcc = value; OnPropertyChanged(); }
        }

        double minVel;
        [DataItem("fpm", "Min Velocity"), Persistent]
        public double MinVel {
            get => minVel;
            set { minVel = value; OnPropertyChanged(); }
        }
        double maxVel;
        [DataItem("fpm", "Max Velocity"), Persistent]
        public double MaxVel {
            get => maxVel;
            set { maxVel = value; OnPropertyChanged(); }
        }
        double defVel;
        [DataItem("fpm", "Default Velocity"), Persistent]
        public double DefVel {
            get => defVel;
            set { defVel = value; OnPropertyChanged(); }
        }

        double minDec;
        [DataItem("fpm2", "Min Deceleration"), Persistent]
        public double MinDec {
            get => minDec;
            set { minDec = value; OnPropertyChanged(); }
        }
        double maxDec;
        [DataItem("fpm2", "Max Deceleration"), Persistent]
        public double MaxDec {
            get => maxDec;
            set { maxDec = value; OnPropertyChanged(); }
        }
        double defDec;
        [DataItem("fpm2", "Default Deceleration"), Persistent]
        public double DefDec {
            get => defDec;
            set { defDec = value; OnPropertyChanged(); }
        }

        double minLoad;
        [DataItem("lbs", "Min Load"), Persistent]
        public double MinLoad {
            get => minLoad;
            set { minLoad = value; OnPropertyChanged(); }
        }
        double maxLoad;
        [DataItem("lbs", "Max Load"), Persistent]
        public double MaxLoad {
            get => maxLoad;
            set { maxLoad = value; OnPropertyChanged(); }
        }
        double loadOffs;
        [DataItem("lbs", "Load Offset"), Persistent]
        public double LoadOffs {
            get => loadOffs;
            set { loadOffs = value; OnPropertyChanged(); }
        }

        double feetPerRotation = 1.0;
        [DataItem("ft", "Feet/Rotation"), Persistent]
        public double FeetPerRotation {
            get => feetPerRotation;
            set { feetPerRotation = value <= 0.0 ? 1.0 : value; OnPropertyChanged(); }
        }
        #endregion
    }
}
