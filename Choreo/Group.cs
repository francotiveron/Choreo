using Choreo.TwinCAT;
using System.Linq;
using System.Windows.Media;
using static Choreo.Globals;

namespace Choreo {
    public partial class Group: Axis {
        public static readonly Color[] GroupColors = { Colors.Red, Colors.Blue, Colors.Purple, Colors.Orange, Colors.DarkCyan, Colors.Green, Colors.Brown, Colors.Magenta };
        public Group(int index) : base(index) { }

        bool save;
        [Plc("GVL.Group_NN_Save")]
        public bool Save
        {
            get => save;
            set { save = !value; Notify()(nameof(Save)); }
        }

        #region UI Properties
        public override Color Color {
            get {
                if (Active) return Colors.Lime;
                return GroupColors[Index];
            }
        }
        public bool IsPreset => VM.Presets.Any(p => p.ContainsGroup(Index));
        #endregion
    }
}
