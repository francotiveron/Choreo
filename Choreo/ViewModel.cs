using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Choreo
{
    using Cfg = Configuration;
    public enum Pages { Home, Cueing, Show };
    public class ViewModel: PropertyChangedNotifier
    {
        public ViewModel() {
            Motors = new List<Motor>();
            Groups = new List<Group>();
            for (int i = 0; i < 16; i++) Motors.Add(new Motor(i));
            for (int i = 0; i < 8; i++) Groups.Add(new Group(i));
            CurrentPage = Pages.Home;
            Plc = PlcFactory.New(Cfg.PLCId); 
        }
        public IPlc Plc { get; private set; }

        Pages currentPage;
        public Pages CurrentPage {
            get => currentPage;
            set { currentPage = value; OnPropertyChanged(); }
        }
        public List<Motor> Motors { get; private set; }
        public List<Group> Groups { get; private set; }

        public void BeginGroupEditing(int group) => GroupBeingEdited = group + 1;
        public void EndGroupEditing() => GroupBeingEdited = 0;
        public void GroupEditSave() {
            var group = Groups[GroupBeingEdited - 1];
            group.Motors.Clear();
            for (int i = 0; i < Motors.Count; i++)
                if (Motors[i].Group == GroupBeingEdited)
                    group.Motors.Add(i);
            Save();
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

        int groupBeingEdited;
        public int GroupBeingEdited {
            get => groupBeingEdited;
            set { groupBeingEdited = value; OnPropertyChanged(); }
        }
        public bool IsGroupEditing => GroupBeingEdited > 0;

        public void Save() { Storage.SaveMotors(); }
        public void Load() { }
    }
}
