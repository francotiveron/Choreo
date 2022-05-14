using Choreo.TwinCAT;
using Choreo.UserManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using static Choreo.Globals;
using static Choreo.Storage;
using static System.Linq.Enumerable;

namespace Choreo
{
    public enum MainWindowPages { Home, Cueing, Show };
    public class ViewModel: PropertyChangedNotifier
    {
        public ViewModel() {
            CurrentMainWindowPage = MainWindowPages.Home;
        }

        public void Init()
        {
            FaultCodeMap = Plc.DownloadErrorMapping();
            Plc.PropertyChanged += Plc_PropertyChanged;
        }

        void Plc_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(AdsPlc.IsOn)) {
                if (Plc.IsOn) {
                    var mg = new ushort[16];
                    Plc.GetMotorsGroup(ref mg);
                    foreach (var m in VM.Motors) m.Group = mg[m.Index];
                }
            }
        }

        public List<Motor> Motors { get; } = new List<Motor>(Range(0, 16).Select(i => new Motor(i)));
        public List<Group> Groups { get; } = new List<Group>(Range(0, 8).Select(i => new Group(i)));
        public IEnumerable<Axis> Axes {
            get {
                foreach (var m in Motors) yield return m;
                foreach (var g in Groups) yield return g;
            }
        }
        public List<Preset> Presets { get; } = new List<Preset>(Range(0, 8).Select(i => new Preset(i)));
        public ObservableCollection<Cue> Cues { get; } = new ObservableCollection<Cue>();
        public IEnumerable<Cue> VisibleCues => Cues.Where(cue => cue.Show);
        public Motion Motion { get; } = new Motion();

        private void CueCompleteChanged() {
            if (CueComplete) {
                if (LoadedCue > 0) {
                    var nextCue = Cues.Skip(LoadedCue).FirstOrDefault(cue => cue.Enabled && cue.IsConsistent);
                    Plc.Upload(nextCue);
                }
            }
        }
        private void PresetCompleteChanged() {
            if (PresetComplete) {
                if (LoadedPreset > 0) {
                    Plc.Upload(default(Preset));
                }
            }
        }

        bool JogOrMoveEnabled => Axes.Any(ax => ax.MREnable || ax.MAEnable || ax.JogUpEnable || ax.JogDnEnable);
        public bool MoveActive {
            get => Motors.Any(ax => ax.Active);
            set => Notify();
        }

        #region Runtime Properties
        private bool cueLoaded;
        [Plc("Cue_loaded")]
        public bool CueLoaded {
            get { return cueLoaded; }
            set { cueLoaded = value; Notify(); }
        }

        bool cueComplete;
        [Plc("Cue_Complete")]
        public bool CueComplete {
            get { return cueComplete; }
            set { cueComplete = value; Notify(); CueCompleteChanged(); }
        }

        private bool presetLoaded;
        [Plc("Preset_loaded")]
        public bool PresetLoaded {
            get { return presetLoaded; }
            set { presetLoaded = value; Notify(); }
        }

        bool presetComplete;
        [Plc("Preset_Complete")]
        public bool PresetComplete {
            get { return presetComplete; }
            set { presetComplete = value; Notify(); PresetCompleteChanged(); }
        }

        bool globalESStatus;
        [Plc("Global_ES_Status")]
        public bool GlobalESStatus {
            get { return globalESStatus; }
            set { globalESStatus = value; Notify(); }
        }

        bool parameterWrite;
        [Plc("Parameter_Write")]
        public bool ParameterWrite {
            get { return parameterWrite; }
            set { parameterWrite = value; Notify(); }
        }

        private double jogVelocity;
        [Plc("Jog_Velocity")]
        public double JogVelocity {
            get { return jogVelocity; }
            set { jogVelocity = value; Notify(); }
        }

        bool deadMan;
        [Plc("Dead_Man")]
        public bool DeadMan
        {
            get { return deadMan; }
            set { deadMan = value; Notify(); }
        }
        #endregion

        #region ******************Motor and Group Settings Editing******************
        int motorSettingsBeingEdited;
        public int MotorSettingsBeingEdited {
            get => motorSettingsBeingEdited;
            set { motorSettingsBeingEdited = value; Notify(); }
        }
        public bool IsMotorSettingsEditing => MotorSettingsBeingEdited > 0;
        public void BeginMotorSettingsEditing(int index) => MotorSettingsBeingEdited = index + 1;
        public void EndMotorSettingsEditing() => MotorSettingsBeingEdited = 0;
        public void MotorSettingsEditCancel() {
            Plc.Download(Motors[MotorSettingsBeingEdited - 1]);
            Load(Motors[MotorSettingsBeingEdited - 1]);
            EndMotorSettingsEditing();
        }

        public void MotorSettingsEditSave() {
            Plc.Upload(Motors[MotorSettingsBeingEdited - 1]);
            Save(Motors[MotorSettingsBeingEdited - 1]);
            EndMotorSettingsEditing();
        }

        int groupSettingsBeingEdited;
        public int GroupSettingsBeingEdited {
            get => groupSettingsBeingEdited;
            set { groupSettingsBeingEdited = value; Notify(); }
        }
        public bool IsGroupSettingsEditing => GroupSettingsBeingEdited > 0;
        public void BeginGroupSettingsEditing(int index) => GroupSettingsBeingEdited = index + 1;
        public void EndGroupSettingsEditing() => GroupSettingsBeingEdited = 0;
        public void GroupSettingsEditCancel() {
            Plc.Download(Groups[GroupSettingsBeingEdited - 1]);
            Load(Groups[GroupSettingsBeingEdited - 1]);
            EndGroupSettingsEditing();
        }
        public void GroupSettingsEditSave() {
            Plc.Upload(Groups[GroupSettingsBeingEdited - 1]);
            Save(Groups[GroupSettingsBeingEdited - 1]);
            EndGroupSettingsEditing();
        }
        #endregion

        #region ******************Group grouping Editing******************
        int groupBeingEdited;
        HashSet<Motor> editedGroupMotorsInitial;
        public int GroupBeingEdited {
            get => groupBeingEdited;
            set { groupBeingEdited = value; Notify(); }
        }
        public bool IsGroupEditing => GroupBeingEdited > 0;
        public void BeginGroupEditing(int group) {
            if (JogOrMoveEnabled) {
                Log.PopWarning("Please clear all Moves and Jogs before editing group", "Operation Conflict");
                return;
            }
            GroupBeingEdited = group + 1;
            editedGroupMotorsInitial = Motors.Where(m => m.Group == GroupBeingEdited).ToHashSet();
        }
        void EndGroupEditing() {
            editedGroupMotorsInitial = null;
            GroupBeingEdited = 0;
        }
        public void GroupEditSave() {
            var group = Groups[GroupBeingEdited - 1];

            ushort bitmap = 0;
            for (ushort i = 0, mask = 1; i < VM.Motors.Count; i++, mask <<= 1)
                if (VM.Motors[i].Group == group.Number)
                    bitmap |= mask;

            if (!Plc.SaveGroupMotors(group.Index, bitmap)) GroupEditCancel();
            EndGroupEditing();
            BeginGroupSettingsEditing(group.Index);
        }
        public void GroupEditClear() { foreach (var m in Motors.Where(m => m.Group == GroupBeingEdited)) m.Group = 0; Groups[GroupBeingEdited - 1].Name = null; }
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
            set { presetBeingEdited = value; Notify()(nameof(IsPresetEditing)); }
        }
        public bool IsPresetEditing => PresetBeingEdited > 0;

        public void BeginPresetEditing(int preset) {
            Presets[preset].Backup();
            PresetBeingEdited = preset + 1;
        }
        public void EndPresetEditing() => PresetBeingEdited = 0;
        public void PresetEditSave() {
            Presets[PresetBeingEdited - 1].Update();
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
            Presets[PresetBeingEdited - 1].Name = null;
        }
        public void PresetEditCancel() {
            Presets[PresetBeingEdited - 1].Restore();
            EndPresetEditing();
        }
        #endregion

        #region ******************Cueing******************
        int cueBeingEdited;
        public int CueBeingEdited {
            get => cueBeingEdited;
            set { cueBeingEdited = value; Notify(); }
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

        #region ***********************Motion***************************

        private bool motionEditing;

        public bool MotionEditing {
            get { return motionEditing; }
            set { motionEditing = value; Notify(); }
        }

        public void BeginMotionEditing(bool relative, Axis hook) {
            Motion.Hook = hook;
            Motion.Relative = relative;
            MotionEditing = true;
        }
        public void EndMotionEditing() {
            MotionEditing = false;
        }

        internal void SaveMotionEditing() {
            Plc.Upload(Motion);
            MotionEditing = false;
        }

        #endregion

        #region Other Properties
        MainWindowPages currentMainWindowPage;
        public MainWindowPages CurrentMainWindowPage {
            get => currentMainWindowPage;
            set { currentMainWindowPage = value; Notify(); }
        }
        public bool IsEditing => IsGroupEditing || IsPresetEditing;

        private int loadedCue;
        public int LoadedCue {
            get { return loadedCue; }
            set { loadedCue = value; Notify(); }
        }

        private int loadedPreset;
        public int LoadedPreset {
            get { return loadedPreset; }
            set { loadedPreset = value; Notify(); }
        }

        public bool IsAdmin {
            get => User.IsAdmin;
            set => Notify();
        }
        #endregion
    }

    [System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    sealed class PersistentAttribute : Attribute {
        public PersistentAttribute() {}
    }
}
