using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Choreo.Globals;

namespace Choreo {
    public static class Storage {
        static RegistryKey root = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Verendus\Choreo\Settings");
        static void Write(string element, string setting, object value) {
            var key = root.CreateSubKey(element);
            key.SetValue(setting, value);
        }
        static object Read(string element, string setting) {
            var key = root.OpenSubKey(element);
            return key?.GetValue(setting);
        }
        public static void Save(Motor motor) {
            Write($@"Motors\[{motor.Index}]", "Group", motor.Group);
        }
        public static void Load(Motor motor) {
            var value = Read($@"Motors\[{motor.Index}]", "Group");
            int group = (int)(value ?? 0);
            motor.Group = group;
        }
        public static void SaveMotors() { foreach (Motor m in VM.Motors) Save(m); }
        public static void LoadMotors() { foreach (Motor m in VM.Motors) Load(m); }
        public static void LoadAll() {
            LoadMotors();
        } 
    }
}
