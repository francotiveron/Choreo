using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using static Choreo.Storage;
using static System.Linq.Enumerable;

namespace Choreo
{
    using Cfg = Configuration;
    public enum Pages { Home, Cueing, Show };
    public class ViewModel: PropertyChangedNotifier
    {
        public ViewModel() {
            CurrentPage = Pages.Home;
            Plc = PlcFactory.New(Cfg.PLCId); 
        }
        public List<Motor> Motors { get; } = new List<Motor>(Range(0, 16).Select(i => new Motor(i)));
        public List<Group> Groups { get; } = new List<Group>(Range(0, 8).Select(i => new Group(i)));
        public List<Preset> Presets { get; } = new List<Preset>(Range(0, 8).Select(i => new Preset(i)));
        public IPlc Plc { get; private set; }

        Pages currentPage;
        public Pages CurrentPage {
            get => currentPage;
            set { currentPage = value; OnPropertyChanged(); }
        }

        public bool IsEditing => IsGroupEditing || IsPresetEditing;

        #region ******************Motor Settings Editing******************
        int motorSettingsBeingEdited;
        public int MotorSettingsBeingEdited {
            get => motorSettingsBeingEdited;
            set { motorSettingsBeingEdited = value; OnPropertyChanged(); }
        }
        public bool IsMotorSettingsEditing => MotorSettingsBeingEdited > 0;
        public void BeginMotorSettingsEditing(int index) => MotorSettingsBeingEdited = index + 1;
        public void EndMotorSettingsEditing() => MotorSettingsBeingEdited = 0;
        #endregion

        #region ******************Group Editing******************
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
        public void EndGroupEditing() {
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

            foreach (var m in motorsToSave) Save(m);
            EndGroupEditing();
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

        List<KeyValuePair<int, float>> presetMotorsBackup;
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
        }
        public void PresetEditCancel() {
            var preset = Presets[PresetBeingEdited - 1];
            preset.MotorPositions.Clear();
            foreach (var kv in presetMotorsBackup) preset.MotorPositions[kv.Key] = kv.Value;
            presetMotorsBackup = null;
            EndPresetEditing();
        }
        #endregion
    }
}
