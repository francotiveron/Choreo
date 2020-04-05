using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static Choreo.Globals;

namespace Choreo
{
    public class Group: PropertyChangedNotifier
    {
        public static readonly Color[] GroupColors = { Colors.Red, Colors.Blue, Colors.Purple, Colors.Orange, Colors.DarkCyan, Colors.Green, Colors.Brown, Colors.Magenta };
        public Group(int index) { Index = index; }

        public bool Contains(int motorIndex) => VM.Motors.Any(m => m.Group == Index);
        public float Position => 32.1F;
        public float Load => 2214F;


        private bool isOK;
        public bool IsOK {
            get => isOK; 
            set { isOK = value; OnPropertyChanged(); }
        }

        private string name;
        public string Name {
            get {
                if (name == null) return $"Group {Index + 1}";
                return name;
            }
            set { name = value; OnPropertyChanged(); }
        }

        public int Index { get; set; }
        public Color Color => GroupColors[Index];
    }
}
