﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using static Choreo.Globals;

namespace Choreo
{
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
        static object Read(RegistryKey key, string setting) => key?.GetValue(setting);
        static object Read(string element, string setting) => Read(root.OpenSubKey(element), setting);
        static void Read(RegistryKey key, object obj) {
            foreach (var pi in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                if (null != pi.GetCustomAttribute<PersistentAttribute>() && pi.CanWrite)
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
        public static void Save(Motor motor) => Write($@"Motors\[{motor.Index}]", motor);
        public static void Load(Motor motor) => Read($@"Motors\[{motor.Index}]", motor);
        public static void SaveMotors() => Save(VM.Motors);
        public static void LoadMotors() => Load(VM.Motors);
        #endregion

        #region Groups
        public static void Save(Group group) => Write($@"Groups\[{group.Index}]", group);
        public static void Load(Group group) => Read($@"Groups\[{group.Index}]", group);
        public static void SaveGroups() => Save(VM.Groups);
        public static void LoadGroups() => Load(VM.Groups);
        #endregion

        #region Presets
        public static void Load(Preset preset) {
            var value = Read($@"Presets\[{preset.Index}]", "Name") as string;
            preset.Name = value;
            value = Read($@"Presets\[{preset.Index}]", "MotorPositions") as string;
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
        public static void Save(Preset preset) {
            Write($@"Presets\[{preset.Index}]", "Name", preset.Name);
            var value = string.Join(";", preset.MotorPositions.Select(kv => $"{kv.Key},{kv.Value}"));
            Write($@"Presets\[{preset.Index}]", "MotorPositions", value);
            value = string.Join(";", preset.GroupPositions.Select(kv => $"{kv.Key},{kv.Value}"));
            Write($@"Presets\[{preset.Index}]", "GroupPositions", value);
        }

        public static void SavePresets() => Save(VM.Presets);
        public static void LoadPresets() => Load(VM.Presets);
        #endregion

        #region Cues
        public static void Save(Cue cue) {
            var elementKey = $@"Cues\{cue.Id}";
            try { root.DeleteSubKeyTree($@"{elementKey}\Rows"); } catch { }

            Write(elementKey, cue);
            Write(elementKey, "Index", cue.Index);
            foreach (var row in cue.Rows) {
                var rowKey = $@"{elementKey}\Rows\{row.Id}";
                Write(rowKey, row);
                Write(rowKey, "Index", row.Index);
            }
        }

        public static void SaveCueEnable(Cue cue)
        {
            var elementKey = $@"Cues\{cue.Id}";

            Write(elementKey, "Enabled", cue.Enabled);
        }

        static void UpdateCueIndexes()
        {
            foreach(var cue in VM.Cues)
            {
                var elementKey = $@"Cues\{cue.Id}";
                Write(elementKey, "Index", cue.Index);
            }
        }

        public static void Load(Cue cue) {
            var elementKey = $@"Cues\{cue.Id}";
            var cueRoot = root.OpenSubKey(elementKey);
            if (cueRoot != null) LoadCue(cue, cueRoot);
        }

        static void LoadCue(Cue cue, RegistryKey cueRoot) {
            Read(cueRoot, cue);
            cue.Rows.Clear();
            var rowsRoot = cueRoot.OpenSubKey("Rows");
            if (rowsRoot == null) return;
            var dict = new SortedDictionary<int, CueRow>();
            foreach (var rowId in rowsRoot.GetSubKeyNames()) {
                var rowRoot = rowsRoot.OpenSubKey(rowId);
                if (rowRoot == null) break;
                var idx = (int)Read(rowRoot, "Index");
                var row = new CueRow(cue, rowId);
                Read(rowRoot, row);
                dict[idx] = row;
                //cue.Rows.Add(row);
            }
            foreach (var row in dict.Values) cue.Rows.Add(row);
        }

        public static void Delete(Cue cue) {
            var elementKey = $@"Cues\{cue.Id}";
            root.DeleteSubKeyTree(elementKey);
            UpdateCueIndexes();
            //Save(VM.Cues);
        }

        //public static void Delete(Cue cue, CueRow row) {
        //    var elementKey = $@"Cues\{cue.Id}\Rows\{row.Id}";
        //    root.DeleteSubKeyTree(elementKey);
        //    Save(cue);
        //}

        //public static void SaveCues() => Save(VM.Cues);
        
        public static void LoadCues() {
            if (root.OpenSubKey("Cues") is RegistryKey cuesRoot) {
                var dict = new SortedDictionary<int, Cue>();
                foreach (var cueId in cuesRoot.GetSubKeyNames()) {
                    var cue = new Cue(cueId);
                    var cueRoot = cuesRoot.OpenSubKey(cueId);
                    if (cueRoot == null) break;
                    var idx = (int)Read(cueRoot, "Index");
                    LoadCue(cue, cueRoot);
                    dict[idx] = cue;
                    //VM.Cues.Add(cue);
                }
                VM.Cues.Clear();
                foreach (var cue in dict.Values) VM.Cues.Add(cue);
            }
        }
        #endregion

        #region Axis Configurations
        public static void Save(AxisConfiguration config) => Write($@"Config\{config.Name}", config);

        public static void DeleteAxisConfiguration(string name) => root.DeleteSubKey($@"Config\{name}");

        public static void Load(AxisConfiguration config) => Read($@"Config\{config.Name}", config);

        public static string[] LoadAxisConfigurationsNames()
        {
            if (root.OpenSubKey("Config") is RegistryKey key)
            {
                return key.GetSubKeyNames();
            }
            return null;
        }
        #endregion

        //public static void SaveAll() {
        //    SaveMotors();
        //    SaveGroups();
        //    SavePresets();
        //    SaveCues();
        //}

        public static void LoadAll() {
            LoadMotors();
            LoadGroups();
            LoadPresets();
            LoadCues();
        }

        public static void PersistToRegistry()
        {
            try
            {
                RegFlushKey(HKEY_CURRENT_USER);
            }
            catch (Exception e)
            {
                Log.PopError(e.Message, "Registry Flush Failed");
            }
        }
        
        [DllImport("Advapi32.dll")]
        static extern int RegFlushKey(IntPtr hKey);

        //const uint HKEY_CURRENT_USER = 0x80000001; 
        private static readonly IntPtr HKEY_CURRENT_USER = new IntPtr(-2147483647);
    }
}
