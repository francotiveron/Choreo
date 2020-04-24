using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static Choreo.Globals;

namespace Choreo
{
    public partial class Group: PropertyChangedNotifier
    {
        public static readonly Color[] GroupColors = { Colors.Red, Colors.Blue, Colors.Purple, Colors.Orange, Colors.DarkCyan, Colors.Green, Colors.Brown, Colors.Magenta };
        public Group(int index) { Index = index; }

        public bool Contains(int motorIndex) => VM.Motors.Any(m => m.Group == Index);

        public double Rotations {
            get => Position / FeetPerRotation;
            set => Position = value * FeetPerRotation;
        }

        double position;
        [DataItem]
        public double Position {
            get => position;
            set { position = value; OnPropertyChanged(); }
        }

        double load;
        [DataItem("lbs")]
        public double Load {
            get => load;
            set { load = value; OnPropertyChanged(); }
        }


        bool isOK;
        public bool IsOK {
            get => isOK; 
            set { isOK = value; OnPropertyChanged(); }
        }

        string name;
        public string Name {
            get {
                if (name == null) return $"Group {Index + 1:00}";
                return name;
            }
            set { name = value; OnPropertyChanged(); }
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
        public int Index { get; set; }
        public Color Color => GroupColors[Index];

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
            set { feetPerRotation = value <= 0.0 ? 1.0: value; OnPropertyChanged(); }
        }
        #endregion
    }
}
