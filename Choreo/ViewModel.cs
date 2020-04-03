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
        #region Group Editing
        int groupBeingEdited;
        public int GroupBeingEdited {
            get => groupBeingEdited;
            set { groupBeingEdited = value; OnPropertyChanged(); }
        }
        public bool IsGroupEditing => GroupBeingEdited > 0;
        public void BeginGroupEditing(int group) => GroupBeingEdited = group + 1;
        public void EndGroupEditing() => GroupBeingEdited = 0;
        public void GroupEditSave() {
            var group = Groups[GroupBeingEdited - 1];
            group.Motors.Clear();
            for (int i = 0; i < Motors.Count; i++)
                if (Motors[i].Group == GroupBeingEdited)
                    group.Motors.Add(i);
            SaveMotors();
            SaveGroups();
            EndGroupEditing();
        }
        public void GroupEditClear() { foreach (Motor m in Motors.Where(m => m.Group == GroupBeingEdited)) m.Group = 0; }
        public void GroupEditCancel() {
            var group = Groups[GroupBeingEdited - 1];
            var groupSet = new HashSet<int>(group.Motors);
            var motorSet = new HashSet<int>(Motors.Where(m => m.Group == GroupBeingEdited).Select(m => m.Index));
            var motorsToSet = groupSet.Except(motorSet);
            var motorsToReset = motorSet.Except(groupSet);

            foreach (int i in motorsToSet) Motors[i].Group = GroupBeingEdited;
            foreach (int i in motorsToReset) Motors[i].Group = 0;
            EndGroupEditing();
        }
        #endregion

        #region Preset Editing
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
            SavePresets();
            EndPresetEditing();
        }
        public void PresetEditClear() { Presets[PresetBeingEdited - 1].Clear(); }
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
