using System.Collections.Generic;
using System.Linq;
using static Choreo.Globals;

namespace Choreo {
    public class Preset : PropertyChangedNotifier 
    {
        public Preset(int index) { Index = index; }
        Preset() { }
        public int Index { get; set; }
        public int Number => Index + 1;

        string name;
        public string Name {
            get => name ?? string.Empty;
            set { name = string.IsNullOrWhiteSpace(value) ? null : value; Notify()(nameof(FullName)); }
        }

        public string FullName {
            get {
                if (name == null) return $"{GetType().Name} {Number:0}";
                return $"{Number:0}-{name}";
            }
        }

        public Dictionary<int, double> MotorPositions = new Dictionary<int, double>();
        public Dictionary<int, double> GroupPositions = new Dictionary<int, double>();

        public bool ContainsMotor(int motorIndex) => MotorPositions.ContainsKey(motorIndex);
        public bool ContainsGroup(int groupIndex) => GroupPositions.ContainsKey(groupIndex);
        static readonly double realisedWindow = 1.0 / 12.0 / 10.0; //a tenth of inch in feets
        public bool Realised { 
            get {
                bool SamePos(double p1, double p2) => System.Math.Abs(p1 - p2) <= realisedWindow;
                return
                    !IsEmpty
                    &&
                    MotorPositions.All(mp => SamePos(mp.Value, VM.Motors[mp.Key].Position))
                    &&
                    GroupPositions.All(gp => SamePos(gp.Value, VM.Groups[gp.Key].Position));
            }
        }
        public bool IsEmpty => MotorPositions.Count == 0 && GroupPositions.Count == 0;
        public bool Toggle(object element) {
            switch(element) {
                case Motor motor:
                    if (ContainsMotor(motor.Index)) MotorPositions.Remove(motor.Index);
                    else MotorPositions[motor.Index] = motor.Position;
                    motor.PresetTouch();
                    return ContainsMotor(motor.Index);

                case Group group:
                    if (ContainsGroup(group.Index)) GroupPositions.Remove(group.Index);
                    else GroupPositions[group.Index] = group.Position;
                    group.PresetTouch();
                    return ContainsGroup(group.Index);
            }
            return false;
        }

        string nameBackup;
        Dictionary<int, double> MotorPositionsBackup = new Dictionary<int, double>();
        Dictionary<int, double> GroupPositionsBackup = new Dictionary<int, double>();
        public void Backup() {
            nameBackup = Name;
            MotorPositionsBackup.Clear();
            foreach (var kv in MotorPositions) MotorPositionsBackup[kv.Key] = kv.Value;
            GroupPositionsBackup.Clear();
            foreach (var kv in GroupPositions) GroupPositionsBackup[kv.Key] = kv.Value;
        }
        public void Restore() {
            MotorPositions.Clear();
            foreach (var kv in MotorPositionsBackup) MotorPositions[kv.Key] = kv.Value;
            GroupPositions.Clear();
            foreach (var kv in GroupPositionsBackup) GroupPositions[kv.Key] = kv.Value;
            Name = nameBackup;
        }

        /*
         * Rewrites actual positions. This is required for the following scenario
         * 1 - Preset is empty
         * 2 - Preset is created => axes are selected in the UI => positions are copied in preset
         * 3 - Axes are moved for normal operation
         * 4 - User opens preset and save => as no touch has been performed, new axes position are not updated in the preset
         * Calling Update before Save fixes this scenario
         */
        public void Update() {
            foreach (var i in MotorPositions.Keys.ToArray()) MotorPositions[i] = VM.Motors[i].Position;
            foreach (var i in GroupPositions.Keys.ToArray()) GroupPositions[i] = VM.Groups[i].Position;
            MultiNotify("Realised");
        }
    }
}
