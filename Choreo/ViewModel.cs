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

        public int GroupBeingEdited => 2;
    }
}
