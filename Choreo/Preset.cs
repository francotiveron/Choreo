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

        private string name;
        public string Name {
            get {
                if (name == null) return $"Preset {Index + 1}";
                return name;
            }
            set { name = value; OnPropertyChanged(); }

        }

        Dictionary<int, float> MotorPositions = new Dictionary<int, float>();

        public bool ContainsMotor(int motorIndex) => MotorPositions.ContainsKey(motorIndex);

        public bool ToggleMotor(Motor motor) {
            if (ContainsMotor(motor.Index))MotorPositions.Remove(motor.Index);
            else MotorPositions[motor.Index] = motor.Position;
            motor.PresetTouch();
            return ContainsMotor(motor.Index);
        }

        public void Clear() => MotorPositions.Clear();
    }
}
