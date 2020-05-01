using Choreo.TwinCAT;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using static Choreo.Globals;

namespace Choreo {
    public class Axis : PropertyChangedNotifier {
        protected void OnStatusChanged([CallerMemberName] string name = null) {
            base.OnPropertyChanged(name);
            //NotifyFurther(nameof(AxisStatus));
        }

        public Axis(int index) { Index = index; }
        #region Runtime+PLC Properties

        double rotations;
        [Plc("Act_Pos")]
        public double Rotations {
            get => rotations;
            set { rotations = value; OnPropertyChanged(nameof(Position)); }
        }

        double calibrationRotations;
        [Plc("Calibration_Value")]
        public double CalibrationRotations {
            get => calibrationRotations;
            set { calibrationRotations = value; OnPropertyChanged(nameof(CalibrationValue)); }
        }

        double load;
        [DataItem("lbs"), Plc]
        public double Load {
            get => load;
            set { load = value; OnPropertyChanged(); }
        }

        public DataItemUI.States LoadStatus => DataItemUI.States.OK;

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

        bool active;
        [Plc]
        public bool Active {
            get => active;
            set { active = value; OnPropertyChanged(); OnPropertyChanged("Color"); }
        }

        bool present;
        [Plc]
        public bool Present {
            get => present;
            set { present = value; OnPropertyChanged(); }
        }

        [DataItem(title: "Set Position")]
        public double CalibrationValue {
            get => CalibrationRotations * FeetPerRotation;
            set { calibrationRotations = value / FeetPerRotation; OnPropertyChanged(); }
        }

        bool calibrationSave;
        [Plc("Calibration_Save")]
        public bool CalibrationSave {
            get => calibrationSave;
            set { calibrationSave = value; OnPropertyChanged(); }
        }

        #endregion

        #region UI Properties
        string name;
        [DataItem(title: "Axis Name"), Persistent]
        public string Name {
            get {
                if (name == null) return $"{GetType().Name} {Number:00}";
                return name;
            }
            set { name = value; OnPropertyChanged(); }
        }

        public int Index { get; private set; }
        public int Number => Index + 1;

        [DataItem]
        public double Position => Rotations * FeetPerRotation;
        public DataItemUI.States PositionStatus => DataItemUI.States.OK;

        public virtual Color Color => Colors.Yellow;

        int presetTouches;
        public int PresetTouches {
            get => presetTouches;
            set {
                presetTouches = value;
                OnPropertyChanged();
            }
        }
        public void PresetTouch() => ++PresetTouches;

        public int Status {
            get { return 0; }
        }


        #endregion

        #region Settings
        //double setPosition;
        //[DataItem(title: "Set Position"), Persistent]
        //public double SetPosition {
        //    get => setPosition;
        //    set { setPosition = value; OnPropertyChanged(); }
        //}

        //public DataItemUI.States SetPositionStatus => DataItemUI.States.OK;

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
        [DataItem("lbs", "Min Load"), Plc("Min_Load")]
        public double MinLoad {
            get => minLoad;
            set { minLoad = value; OnPropertyChanged(); }
        }
        double maxLoad;
        [DataItem("lbs", "Max Load"), Plc("Max_Load")]
        public double MaxLoad {
            get => maxLoad;
            set { maxLoad = value; OnPropertyChanged(); }
        }
        double loadOffs;
        [DataItem("lbs", "Load Offset"), Plc("Load_Offset")]
        public double LoadOffs {
            get => loadOffs;
            set { loadOffs = value; OnPropertyChanged(); }
        }

        double feetPerRotation = 1.0;
        [DataItem("ft", "Feet/Rotation"), Persistent]
        public double FeetPerRotation {
            get => feetPerRotation;
            set { 
                feetPerRotation = value <= 0.0 ? 1.0 : value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Position));
                OnPropertyChanged(nameof(CalibrationValue));
            }
        }
        #endregion

    }
}
