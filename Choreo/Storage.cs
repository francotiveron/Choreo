using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public static void Save(Preset preset) {
            var value = string.Join(";", preset.MotorPositions.Select(kv => $"{kv.Key},{kv.Value}"));
            Write($@"Presets\[{preset.Index}]", "MotorPositions", value);
        }
        public static void Load(Motor motor) {
            var value = Read($@"Motors\[{motor.Index}]", "Group");
            if (value is int group) motor.Group = group;
        }
        public static void Load(Preset preset) {
            var value = Read($@"Presets\[{preset.Index}]", "MotorPositions");
            if (value is string s) {
                var kvs = s.Split(';').Select(kv => kv.Split(',')).Select(kv => (int.Parse(kv[0]), float.Parse(kv[1])));
                preset.MotorPositions.Clear();
                foreach (var (index, position) in kvs) preset.MotorPositions[index] = position;
            }
        }
        static void Iterate<T>(IEnumerable<T> collection, MethodInfo action) { foreach (var item in collection) action.Invoke(null, new object[] { item }); }
        public static void Save<T>(IEnumerable<T> collection) {
            var saver = typeof(Storage).GetMethod("Save", new Type[] { typeof(T) });
            Iterate(collection, saver);
        }
        public static void Load<T>(IEnumerable<T> collection) {
            var loader = typeof(Storage).GetMethod("Load", new Type[] { typeof(T) });
            Iterate(collection, loader);
        }
        public static void SaveMotors() => Save(VM.Motors);
        public static void LoadMotors() => Load(VM.Motors);
        public static void SavePresets() => Save(VM.Presets);
        public static void LoadPresets() => Load(VM.Presets);
        public static void SaveAll() {
            SaveMotors();
            SavePresets();
        }
        public static void LoadAll() {
            LoadMotors();
            LoadPresets();
        } 
    }
}
