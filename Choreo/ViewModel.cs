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
    public enum Pages { Home };
    public class ViewModel: PropertyChangedNotifier
    {
        public ViewModel() {
            Motors = new List<Motor> { new Motor() };
            CurrentPage = Pages.Home;
            Plc = PlcFactory.New(Cfg.PLCId); 
        }
        public IPlc Plc { get; private set; }
        public Pages CurrentPage { get; set; }
        public List<Motor> Motors { get; private set; }
    }
}
