using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Choreo {
    public class Preset : PropertyChangedNotifier 
    {
        public Preset(int index) { Index = index; }
        public int Index { get; set; }
        public int Number => Index + 1;

        private string name;
        public string Name {
            get {
                if (name == null) return $"Preset {Index + 1}";
                return name;
            }
            set { name = value; Notify(); }

        }

        public Dictionary<int, double> MotorPositions = new Dictionary<int, double>();
        public Dictionary<int, double> GroupPositions = new Dictionary<int, double>();

        public bool ContainsMotor(int motorIndex) => MotorPositions.ContainsKey(motorIndex);
        public bool ContainsGroup(int groupIndex) => GroupPositions.ContainsKey(groupIndex);

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
    }
}
