using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Choreo.Storage;
using static System.Linq.Enumerable;

namespace Choreo
{
    using Cfg = Configuration;
    public enum MainWindowPages { Home, Cueing, Show };
    public class ViewModel: PropertyChangedNotifier
    {
        public ViewModel() {
            CurrentMainWindowPage = MainWindowPages.Home;
            Plc = PlcFactory.New(Cfg.PLCId); 
        }
        public List<Motor> Motors { get; } = new List<Motor>(Range(0, 16).Select(i => new Motor(i)));
        public List<Group> Groups { get; } = new List<Group>(Range(0, 8).Select(i => new Group(i)));
        public List<Preset> Presets { get; } = new List<Preset>(Range(0, 8).Select(i => new Preset(i)));
        public ObservableCollection<Cue> Cues { get; } = new ObservableCollection<Cue>();
        public Motion Motion { get; } = new Motion();
        public IPlc Plc { get; private set; }

        MainWindowPages currentMainWindowPage;
        public MainWindowPages CurrentMainWindowPage {
            get => currentMainWindowPage;
            set { currentMainWindowPage = value; OnPropertyChanged(); }
        }

        public bool IsEditing => IsGroupEditing || IsPresetEditing;

        #region ******************Motor and Group Settings Editing******************
        int motorSettingsBeingEdited;
        public int MotorSettingsBeingEdited {
            get => motorSettingsBeingEdited;
            set { motorSettingsBeingEdited = value; OnPropertyChanged(); }
        }
        public bool IsMotorSettingsEditing => MotorSettingsBeingEdited > 0;
        public void BeginMotorSettingsEditing(int index) => MotorSettingsBeingEdited = index + 1;
        public void EndMotorSettingsEditing() => MotorSettingsBeingEdited = 0;
        public void MotorSettingsEditCancel() {
            Load(Motors[MotorSettingsBeingEdited - 1]);
            EndMotorSettingsEditing();
        }
        public void MotorSettingsEditSave() {
            Save(Motors[MotorSettingsBeingEdited - 1]);
            EndMotorSettingsEditing();
        }

        int groupSettingsBeingEdited;
        public int GroupSettingsBeingEdited {
            get => groupSettingsBeingEdited;
            set { groupSettingsBeingEdited = value; OnPropertyChanged(); }
        }
        public bool IsGroupSettingsEditing => GroupSettingsBeingEdited > 0;
        public void BeginGroupSettingsEditing(int index) => GroupSettingsBeingEdited = index + 1;
        public void EndGroupSettingsEditing() => GroupSettingsBeingEdited = 0;
        public void GroupSettingsEditCancel() {
            Load(Groups[GroupSettingsBeingEdited - 1]);
            EndGroupSettingsEditing();
        }
        public void GroupSettingsEditSave() {
            Save(Groups[GroupSettingsBeingEdited - 1]);
            EndGroupSettingsEditing();
        }
        #endregion

        #region ******************Group grouping Editing******************
        int groupBeingEdited;
        HashSet<Motor> editedGroupMotorsInitial;
        public int GroupBeingEdited {
            get => groupBeingEdited;
            set { groupBeingEdited = value; OnPropertyChanged(); }
        }
        public bool IsGroupEditing => GroupBeingEdited > 0;
        public void BeginGroupEditing(int group) {
            GroupBeingEdited = group + 1;
            editedGroupMotorsInitial = Motors.Where(m => m.Group == GroupBeingEdited).ToHashSet();
        }
        void EndGroupEditing() {
            editedGroupMotorsInitial = null;
            GroupBeingEdited = 0;
        }
        public void GroupEditSave() {
            var group = Groups[GroupBeingEdited - 1];
            var motorsToSave =
                from m in Motors
                where 
                    m.Group == GroupBeingEdited && !editedGroupMotorsInitial.Contains(m)
                    ||
                    m.Group == 0 && editedGroupMotorsInitial.Contains(m)
                select m;

            foreach (var m in motorsToSave) SaveGroup(m);
            EndGroupEditing();
            BeginGroupSettingsEditing(group.Index);
        }
        public void GroupEditClear() { foreach (var m in Motors.Where(m => m.Group == GroupBeingEdited)) m.Group = 0; }
        public void GroupEditCancel() {
            var group = Groups[GroupBeingEdited - 1];
            var editedGroupMotorsCurrent = new HashSet<Motor>(Motors.Where(m => m.Group == GroupBeingEdited));
            var motorsToReset = editedGroupMotorsCurrent.Except(editedGroupMotorsInitial);
            var motorsToSet = editedGroupMotorsInitial.Except(editedGroupMotorsCurrent);

            foreach (var m in motorsToSet) m.Group = GroupBeingEdited;
            foreach (var m in motorsToReset) m.Group = 0;
            EndGroupEditing();
        }
        #endregion

        #region ******************Preset Editing******************
        int presetBeingEdited;
        public int PresetBeingEdited {
            get => presetBeingEdited;
            set { presetBeingEdited = value; OnPropertyChanged(); }
        }
        public bool IsPresetEditing => PresetBeingEdited > 0;

        List<KeyValuePair<int, double>> presetMotorsBackup;
        public void BeginPresetEditing(int preset) {
            PresetBeingEdited = preset + 1;
            presetMotorsBackup = Presets[preset].MotorPositions.ToList();
        }
        public void EndPresetEditing() => PresetBeingEdited = 0;
        public void PresetEditSave() {
            Save(Presets[PresetBeingEdited - 1]);
            EndPresetEditing();
        }
        public void PresetEditClear() {
            var keys = Presets[PresetBeingEdited - 1].MotorPositions.Keys.ToList();
            Presets[PresetBeingEdited - 1].MotorPositions.Clear();
            foreach (var key in keys) Motors[key].PresetTouch();
            keys = Presets[PresetBeingEdited - 1].GroupPositions.Keys.ToList();
            Presets[PresetBeingEdited - 1].GroupPositions.Clear();
            foreach (var key in keys) Groups[key].PresetTouch();
        }
        public void PresetEditCancel() {
            var preset = Presets[PresetBeingEdited - 1];
            preset.MotorPositions.Clear();
            foreach (var kv in presetMotorsBackup) preset.MotorPositions[kv.Key] = kv.Value;
            presetMotorsBackup = null;
            EndPresetEditing();
        }
        #endregion

        #region ******************Cueing******************
        int cueBeingEdited;
        public int CueBeingEdited {
            get => cueBeingEdited;
            set { cueBeingEdited = value; OnPropertyChanged(); }
        }
        public void BeginCueEditing(int cue) {
            CueBeingEdited = cue + 1;
        }
        public void EndCueEditing() => CueBeingEdited = 0;
        public void CueEditSave() {
            Save(Cues[CueBeingEdited - 1]);
            EndCueEditing();
        }
        public void CueEditCancel() {
            Load(Cues[CueBeingEdited - 1]);
            EndCueEditing();
        }
        public void DeleteCue(int cueIndex) {
            var cue = Cues[cueIndex];
            Delete(cue);
            Cues.Remove(cue);
            foreach (var c in Cues) c.RefreshIndex();
        }
        internal void DeleteCueRow(int rowIndex) {
            var cue = Cues[CueBeingEdited -1];
            var row = cue.Rows[rowIndex];
            //Delete(cue, row);
            cue.Rows.Remove(row);
        }
        #endregion

        #region Motion

        private bool motionEditing;

        public bool MotionEditing {
            get { return motionEditing; }
            set { motionEditing = value; OnPropertyChanged(); }
        }

        public void BeginMotionEditing(bool relative, object hook) {
            Motion.Hook = hook;
            Motion.Relative = relative;
            MotionEditing = true;
        }
        public void EndMotionEditing() {
            MotionEditing = false;
        }
        #endregion
    }

    [System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    sealed class PersistentAttribute : Attribute {
        public PersistentAttribute() {}
    }
}
