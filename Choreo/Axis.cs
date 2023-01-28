using Choreo.TwinCAT;
using System.Windows.Media;
using static Choreo.Globals;

namespace Choreo
{
    public class Axis : PropertyChangedNotifier {
        public Axis(int index) { Index = index; }
        public bool IsGroup => this is Group;
        public bool IsMotor => this is Motor;
        //public virtual Status AxisStatus => FaultCode != 0;
        public virtual Status AxisStatus => new Status(FaultCode);
        public virtual string AxisStatusDescription
        {
            get {
                if (FaultCode != 0) return FaultDescription;
                if (MAEnable || MREnable) {
                    return $"{(MAEnable ? "A" : "R")}: {FeetInchesConvert.ToString(MoveVal)}";
                }
                if (JogUpEnable) return "Jog Up";
                if (JogDnEnable) return "Jog Dn";
                if (JogStickEnable) return "Jog Stick";
                return AxisStatus.ToString();
            }
        }

        public bool IsOperational => Present && UserEnable;

        #region Runtime+PLC Properties

        double rotations;
        [Plc("Act_Pos")]
        public double Rotations {
            get => rotations;
            set { rotations = value; Notify()(nameof(Position)); }
        }
        [DataItem]
        public double Position => Rotations / RotationsPerEU;
        public Status PositionStatus => Status.Ok;

        private double posisionSlider;
        [Plc("Position_Slider")]
        public double PositionSlider {
            get { return posisionSlider; }
            set { posisionSlider = value; Notify(); }
        }

        double load;
        [DataItem("lbs"), Plc()]
        public double Load {
            get => load;
            set { load = value; Notify(); }
        }
        public Status LoadStatus => Status.Ok;

        double moveValRotations;
        [Plc("Move_Val")]
        public double MoveValRotations {
            get => moveValRotations;
            set { moveValRotations = value; Notify()(nameof(MoveVal)); }
        }
        public double MoveVal => MoveValRotations / RotationsPerEU;

        double accel;
        [Plc(adsNotify: false)]
        public double Accel {
            get => accel;
            set { accel = value; Notify(); }
        }

        double decel;
        [Plc(adsNotify: false)]
        public double Decel {
            get => decel;
            set { decel = value; Notify(); }
        }

        double velocity;
        [Plc(adsNotify: false)]
        public double Velocity {
            get => velocity;
            set { velocity = value; Notify(); }
        }

        double minVel;
        [DataItem("fpm", "Min(Velocity)"), Plc("Min_Velocity", false)]
        public double MinVel {
            get => minVel;
            set { minVel = value; Notify()(nameof(DefVel), nameof(MaxVel)); }
        }
        public virtual Status MinVelStatus => MinVel < 0 || MinVel > DefVel;

        double maxVel;
        [DataItem("fpm", "Max"), Plc("Max_Velocity", false)]
        public double MaxVel {
            get => maxVel;
            set { maxVel = value; Notify()(nameof(MinVel), nameof(DefVel)); }
        }
        public virtual Status MaxVelStatus => MaxVel < DefVel;

        double defVel;
        [DataItem("fpm", "Default"), Persistent]
        public double DefVel
        {
            get => defVel;
            set { defVel = value; Notify()(nameof(MinVel), nameof(MaxVel)); }
        }
        public Status DefVelStatus => DefVel < MinVel || DefVel > MaxVel;

        bool mAEnable;
        [Plc("MA_Enable")]
        public bool MAEnable {
            get => mAEnable;
            set { mAEnable = value; Notify(); }
        }

        bool mREnable;
        [Plc("MR_Enable")]
        public bool MREnable {
            get => mREnable;
            set { mREnable = value; Notify(); }
        }

        bool jogUpEnable;
        [Plc("Jog_Up_Enable")]
        public bool JogUpEnable {
            get => jogUpEnable;
            set { jogUpEnable = value; Notify(); }
        }


        bool jogDnEnable;
        [Plc("Jog_Dn_Enable")]
        public bool JogDnEnable {
            get => jogDnEnable;
            set { jogDnEnable = value; Notify(); }
        }

        bool overLoad;
        [Plc("Over_Load")]
        public bool OverLoad {
            get => overLoad;
            set { overLoad = value; Notify(); }
        }

        bool underLoad;
        [Plc("Under_Load")]
        public bool UnderLoad {
            get => underLoad;
            set { underLoad = value; Notify(); }
        }

        bool moveComplete;
        [Plc("Move_Complete")]
        public bool MoveComplete {
            get => moveComplete;
            set { moveComplete = value; Notify(); }
        }

        bool userEnable;
        [Plc("User_Enable", false)]
        public bool UserEnable {
            get => userEnable;
            set { userEnable = value; Notify(); }
        }

        bool loadCellActive;
        [Plc("Load_Cell_Enable", false)]
        public bool LoadCellActive
        {
            get => loadCellActive;
            set { loadCellActive = value; Notify(); }
        }

        bool softLimitEnable;
        [Plc("Soft_Limit_Enable", false)]
        public bool SoftLimitEnable
        {
            get => softLimitEnable;
            set { softLimitEnable = value; Notify(); }
        }

        bool jogStickEnable;
        [Plc("Jog_Stick_Enable")]
        public bool JogStickEnable
        {
            get => jogStickEnable;
            set { jogStickEnable = value; Notify(); }
        }

        bool active;
        [Plc]
        public bool Active {
            get => active;
            set { active = value; Notify()(nameof(Color)); VM.MoveActive = !VM.MoveActive; }
        }

        public virtual bool Present { get; protected set; } = true;

        double calibrationRotations;
        [Plc("Calibration_Value", false)]
        public double CalibrationRotations {
            get => calibrationRotations;
            set { calibrationRotations = value; Notify(nameof(CalibrationValue)); }
        }

        [DataItem(title: "Set Position")]
        public double CalibrationValue {
            get => CalibrationRotations / RotationsPerEU;
            set { calibrationRotations = value * RotationsPerEU; Notify(); }
        }

        bool calibrationSave;
        [Plc("Calibration_Save")]
        public bool CalibrationSave {
            get => calibrationSave;
            set { calibrationSave = value; Notify(); }
        }

        ushort faultCode;
        [Plc("Fault_Code")]
        public ushort FaultCode
        {
            get => faultCode;
            set { faultCode = value; Notify()(nameof(AxisStatus)); }
        }

        private string FaultDescription
        {
            get
            {
                if (FaultCode == 0xFFFF) return "Unknown Fault (PLC)";
                if (FaultCodeMap.TryGetValue(FaultCode, out var description))
                {
                    return description;
                }
                return "Unknown Fault (UI)";
            }
        }

        #endregion

        #region UI Properties
        string name;
        [DataItem(title: "Axis Name"), Persistent]
        public string Name {
            get => name ?? string.Empty;
            set { name = string.IsNullOrWhiteSpace(value) ? null : value; Notify()(nameof(FullName)); }
        }

        public string FullName {
            get {
                if (name == null) return $"{GetType().Name} {Number:00}";
                return $"{Number:00}-{name}";
            }
        }

        public int Index { get; private set; }
        public int Number => Index + 1;

        public virtual Color Color => Colors.Yellow;

        int presetTouches;
        public int PresetTouches {
            get => presetTouches;
            set {
                presetTouches = value;
                Notify();
            }
        }
        public void PresetTouch() => ++PresetTouches;

        public virtual bool IsGrouped => false;
        public bool IsUngrouped => !IsGrouped;

        #endregion

        #region Settings

        double softUpRotations;
        [Plc("Soft_Up", false)]
        public double SoftUpRotations {
            get => softUpRotations;
            set { softUpRotations = value; Notify()(nameof(SoftUp)); }
        }

        [DataItem(title: "Soft Up/Max Limit")]
        public double SoftUp {
            get => softUpRotations / RotationsPerEU;
            set { softUpRotations = value * RotationsPerEU; Notify()(nameof(SoftDnStatus)); }
        }
        public Status SoftUpStatus => SoftUp < SoftDn;

        double softDnRotations;
        [Plc("Soft_Down", false)]
        public double SoftDnRotations {
            get => softDnRotations;
            set { softDnRotations = value; Notify()(nameof(SoftDn)); }
        }

        [DataItem(title: "Soft Down/Min Limit")]
        public double SoftDn {
            get => softDnRotations / RotationsPerEU;
            set { softDnRotations = value * RotationsPerEU; Notify()(nameof(SoftUpStatus)); }
        }
        public Status SoftDnStatus => SoftDn > SoftUp;

        [DataItem("fpm2", "Min(Accel.)")]
        public double MinAcc => 1.0;
        public Status MinAccStatus => MinAcc < 0;

        double maxAcc;
        [DataItem("fpm2", "Max"), Plc("Max_Accel", false)]
        public double MaxAcc {
            get => maxAcc;
            set { maxAcc = value; Notify()(nameof(DefAcc)); }
        }
        public virtual Status MaxAccStatus => MaxAcc < DefAcc;

        double defAcc;
        [DataItem("fpm2", "Default"), Persistent]
        public double DefAcc {
            get => defAcc;
            set { defAcc = value; Notify()(nameof(MaxAcc)); }
        }
        public Status DefAccStatus => DefAcc < MinAcc || DefAcc > MaxAcc;

        double jogAcc;
        [DataItem("fpm2", "Jog(Accel.)"), Plc("Jog_Accel", false)]
        public double JogAcc
        {
            get => jogAcc;
            set { jogAcc = value; Notify(); }
        }
        public Status JogAccStatus => Status.Ok;

        [DataItem("fpm2", "Min(Decel.)")]
        public double MinDec => 1.0;
        public Status MinDecStatus => MinDec < 0;

        double maxDec;
        [DataItem("fpm2", "Max"), Plc("Max_Decel", false)]
        public double MaxDec {
            get => maxDec;
            set { maxDec = value; Notify()(nameof(DefDec)); }
        }
        public virtual Status MaxDecStatus => MaxDec < DefDec;

        double defDec;
        [DataItem("fpm2", "Default"), Persistent]
        public double DefDec {
            get => defDec;
            set { defDec = value; Notify()(nameof(MaxDec)); }
        }
        public Status DefDecStatus => DefDec < MinDec || DefDec > MaxDec;

        double jogDec;
        [DataItem("fpm2", "Jog(Decel.)"), Plc("Jog_Decel", false)]
        public double JogDec
        {
            get => jogDec;
            set { jogDec = value; Notify(); }
        }
        public Status JogDecStatus => Status.Ok;

        double minLoad;
        [DataItem("lbs", "Min(Load)"), Plc("Min_Load", false)]
        public double MinLoad {
            get => minLoad;
            set { minLoad = value; Notify()(nameof(MaxLoad), nameof(LoadOffs)); }
        }
        public Status MinLoadStatus => MinLoad > MaxLoad;

        double maxLoad;
        [DataItem("lbs", "Max"), Plc("Max_Load", false)]
        public double MaxLoad {
            get => maxLoad;
            set { maxLoad = value; Notify()(nameof(MinLoad), nameof(LoadOffs)); }
        }
        public Status MaxLoadStatus => MaxLoad < MinLoad;

        double loadOffs;
        [DataItem("lbs", "Offset"), Plc("Load_Offset", false)]
        public double LoadOffs {
            get => loadOffs;
            set { loadOffs = value; Notify()(nameof(MaxLoad), nameof(MinLoad)); }
        }
        public Status LoadOffsStatus => Status.Ok;


        double rotationsPerEU = 1.0;
        
        [DataItem("r/eu", "Rotations/Eng.Unit")]
        public virtual double RotationsPerEU
        {
            get => rotationsPerEU;

            set
            {
                var calVal = CalibrationValue;
                var softUp = SoftUp;
                var softDn = SoftDn;
                rotationsPerEU = value <= 0.0 ? 1.0 : value;
                CalibrationValue = calVal;
                SoftUp = softUp;
                SoftDn = softDn;
                Notify()(nameof(Position), nameof(CalibrationValue), nameof(SoftUp), nameof(SoftDn));
            }
        }
        #endregion

        int group;
        [DataItem(title: "Group")]
        public int Group {
            get => group;
            set {
                group = value;
                Notify()("Color");
            }
        }
    }
}
