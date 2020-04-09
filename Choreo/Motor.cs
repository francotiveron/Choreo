﻿using System;
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
    
    public class Motor: PropertyChangedNotifier
    {
        public Motor(int index) { Index = index; }

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
        [DataItem(title:"Axis Name")]
        public string Name {
            get {
                if (name == null) return $"Motor {Index + 1}";
                return name;
            }
            set { name = value; OnPropertyChanged(); }
        }

        public int Index { get; set; }

        int group;
        [DataItem(title: "Axis Group", edit: true)]
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
                if (group == 0) return Colors.Gray;
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

        #region Settings
        double setPosition;
        [DataItem(edit:true)]
        public double SetPosition {
            get => setPosition;
            set { setPosition = value; OnPropertyChanged(); }
        }
        double softUp;
        [DataItem(title:"Soft Up/Max Limit", edit: true)]
        public double SoftUp {
            get => softUp;
            set { softUp = value; OnPropertyChanged(); }
        }
        double softDn;
        [DataItem(title: "Soft Down/Min Limit", edit: true)]
        public double SoftDn {
            get => softDn;
            set { softDn = value; OnPropertyChanged(); }
        }
        double minAcc;
        [DataItem("fpm2", "Min Acceleration")]
        public double MinAcc {
            get => minAcc;
            set { minAcc = value; OnPropertyChanged(); }
        }
        double maxAcc;
        [DataItem("fpm2", "Max Acceleration", true)]
        public double MaxAcc {
            get => maxAcc;
            set { maxAcc = value; OnPropertyChanged(); }
        }
        double defAcc;
        [DataItem("fpm2", "Default Acceleration", true)]
        public double DefAcc {
            get => defAcc;
            set { defAcc = value; OnPropertyChanged(); }
        }

        double minVel;
        [DataItem("fpm", "Min Velocity")]
        public double MinVel {
            get => minVel;
            set { minVel = value; OnPropertyChanged(); }
        }
        double maxVel;
        [DataItem("fpm", "Max Velocity", true)]
        public double MaxVel {
            get => maxVel;
            set { maxVel = value; OnPropertyChanged(); }
        }
        double defVel;
        [DataItem("fpm", "Default Velocity", true)]
        public double DefVel {
            get => defVel;
            set { defVel = value; OnPropertyChanged(); }
        }

        double minDec;
        [DataItem("fpm2", "Min Deceleration")]
        public double MinDec {
            get => minDec;
            set { minDec = value; OnPropertyChanged(); }
        }
        double maxDec;
        [DataItem("fpm2", "Max Deceleration", true)]
        public double MaxDec {
            get => maxDec;
            set { maxDec = value; OnPropertyChanged(); }
        }
        double defDec;
        [DataItem("fpm2", "Default Deceleration", true)]
        public double DefDec {
            get => defDec;
            set { defDec = value; OnPropertyChanged(); }
        }

        double minLoad;
        [DataItem("lbs", "Min Load")]
        public double MinLoad {
            get => minLoad;
            set { minLoad = value; OnPropertyChanged(); }
        }
        double maxLoad;
        [DataItem("lbs", "Max Load", true)]
        public double MaxLoad {
            get => maxLoad;
            set { maxLoad = value; OnPropertyChanged(); }
        }
        double loadOffs;
        [DataItem("lbs", "Load Offset", true)]
        public double LoadOffs {
            get => loadOffs;
            set { loadOffs = value; OnPropertyChanged(); }
        }

        double rotations;
        [DataItem("lbs", "Motor Rotations")]
        public double Rotations {
            get => rotations;
            set { rotations = value; OnPropertyChanged(); }
        }
        #endregion
    }
}
