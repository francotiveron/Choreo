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
        static RegistryKey root = Registry.CurrentUser.CreateSubKey($@"SOFTWARE\Verendus\Choreo\{Assembly.GetExecutingAssembly().GetName().Version}\Settings");
        static void Write(string element, string setting, object value) {
            var key = root.CreateSubKey(element);
            if (value != null) key.SetValue(setting, value);
        }
        static void Write(RegistryKey key, object obj) {
            foreach (var pi in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                if (null != pi.GetCustomAttribute<PersistentAttribute>())
                    if (pi.GetValue(obj) is object value && value != null)
                        key.SetValue(pi.Name, value);
        }
        static void Write(string element, object obj) {
            var key = root.CreateSubKey(element);
            Write(key, obj);
        }
        static object Read(string element, string setting) {
            var key = root.OpenSubKey(element);
            return key?.GetValue(setting);
        }
        static void Read(RegistryKey key, object obj) {
            foreach (var pi in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                if (null != pi.GetCustomAttribute<PersistentAttribute>())
                    if (key.GetValue(pi.Name) is object value && value != null) {
                        var typed = Convert.ChangeType(value, pi.PropertyType);
                        pi.SetValue(obj, typed);
                    }
        }
        static void Read(string element, object obj) {
            if (root.OpenSubKey(element) is RegistryKey key) Read(key, obj);
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

        #region Motors
        public static void SaveGroup(Motor motor) {
            Write($@"Motors\[{motor.Index}]", "Group", motor.Group);
        }
        public static void LoadGroup(Motor motor) {
            var value = Read($@"Motors\[{motor.Index}]", "Group");
            if (value is int group) motor.Group = group;
        }
        public static void Save(Motor motor) => Write($@"Motors\[{motor.Index}]", motor);
        public static void Load(Motor motor) => Read($@"Motors\[{motor.Index}]", motor);
        public static void SaveMotors() => Save(VM.Motors);
        public static void LoadMotors() => Load(VM.Motors);
        #endregion

        #region Groups
        public static void Save(Group group) => Write($@"Groups\[{group.Index}]", group);
        public static void Load(Group group) => Read($@"Groups\[{group.Index}]", group);
        public static void Save(Preset preset) {
            var value = string.Join(";", preset.MotorPositions.Select(kv => $"{kv.Key},{kv.Value}"));
            Write($@"Presets\[{preset.Index}]", "MotorPositions", value);
            value = string.Join(";", preset.GroupPositions.Select(kv => $"{kv.Key},{kv.Value}"));
            Write($@"Presets\[{preset.Index}]", "GroupPositions", value);
        }
        public static void SaveGroups() => Save(VM.Groups);
        public static void LoadGroups() => Load(VM.Groups);
        #endregion

        #region Presets
        public static void Load(Preset preset) {
            var value = Read($@"Presets\[{preset.Index}]", "MotorPositions") as string;
            if (value is string && !string.IsNullOrWhiteSpace(value)) {
                var kvs = value.Split(';').Select(kv => kv.Split(',')).Select(kv => (int.Parse(kv[0]), double.Parse(kv[1])));
                preset.MotorPositions.Clear();
                foreach (var (index, position) in kvs) preset.MotorPositions[index] = position;
            }
            value = Read($@"Presets\[{preset.Index}]", "GroupPositions") as string;
            if (value is string && !string.IsNullOrWhiteSpace(value)) {
                var kvs = value.Split(';').Select(kv => kv.Split(',')).Select(kv => (int.Parse(kv[0]), double.Parse(kv[1])));
                preset.GroupPositions.Clear();
                foreach (var (index, position) in kvs) preset.GroupPositions[index] = position;
            }
        }
        public static void SavePresets() => Save(VM.Presets);
        public static void LoadPresets() => Load(VM.Presets);
        #endregion

        #region Cues
        public static void Save(Cue cue) {
            var elementKey = $@"Cues\{cue.Id}";
            Write(elementKey, cue);
            foreach(var row in cue.Rows) {
                var rowKey = $@"{elementKey}\Rows\{row.Id}";
                Write(rowKey, row);
            }
        }
        public static void Load(Cue cue) {
            var elementKey = $@"Cues\{cue.Id}";
            var cueRoot = root.OpenSubKey(elementKey);
            if (cueRoot != null) LoadCue(cue, cueRoot);
        }
        static void LoadCue(Cue cue, RegistryKey cueRoot) {
            Read(cueRoot, cue);
            var rowsRoot = cueRoot.OpenSubKey("Rows");
            cue.Rows.Clear();
            if (rowsRoot == null) return;
            foreach (var rowId in rowsRoot.GetSubKeyNames()) {
                var rowRoot = rowsRoot.OpenSubKey(rowId);
                if (rowRoot == null) break;
                var row = new CueRow(cue, rowId);
                Read(rowRoot, row);
                cue.Rows.Add(row);
            }

            //var value = cueRoot.GetValue("Name");
            //cue.name = value as string;
            //var rowsRoot = cueRoot.OpenSubKey("Rows");
            //cue.Rows.Clear();
            //if (rowsRoot == null) return;
            //foreach (var rowId in rowsRoot.GetSubKeyNames()) {
            //    var rowRoot = rowsRoot.OpenSubKey(rowId);
            //    if (rowRoot == null) break;
            //    var row = new CueRow(cue, rowId);
            //    var target = double.Parse((string)rowRoot.GetValue("Target"));
            //    row.Target = target;
            //    cue.Rows.Add(row);
            //}
        }
        public static void Delete(Cue cue) {
            var elementKey = $@"Cues\{cue.Id}";
            root.DeleteSubKeyTree(elementKey);
        }
        public static void Delete(Cue cue, CueRow row) {
            var elementKey = $@"Cues\{cue.Id}\Rows\{row.Id}";
            root.DeleteSubKeyTree(elementKey);
        }
        public static void SaveCues() => Save(VM.Cues);
        public static void LoadCues() {
            VM.Cues.Clear();
            if (root.OpenSubKey("Cues") is RegistryKey cuesRoot) {
                foreach (var cueId in cuesRoot.GetSubKeyNames()) {
                    var cue = new Cue(cueId);
                    var cueRoot = cuesRoot.OpenSubKey(cueId);
                    LoadCue(cue, cueRoot);
                    VM.Cues.Add(cue);
                }
            }
        }
        #endregion

        public static void SaveAll() {
            SaveMotors();
            SaveGroups();
            SavePresets();
            SaveCues();
        }
        public static void LoadAll() {
            LoadMotors();
            LoadGroups();
            LoadPresets();
            LoadCues();
        } 
    }
}
