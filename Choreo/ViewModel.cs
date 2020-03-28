using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Choreo
{
    public enum Pages { MainPage };
    public class ViewModel
    {
        public Pages CurrentPage { get; set; }
        public List<Motor> Motors { get; private set; }
        public ViewModel() {
            Motors = new List<Motor> { new Motor() };
            CurrentPage = Pages.MainPage;
        }
    }
}
