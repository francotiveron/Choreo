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
    public partial class Group: PropertyChangedNotifier
    {
        public static readonly Color[] GroupColors = { Colors.Red, Colors.Blue, Colors.Purple, Colors.Orange, Colors.DarkCyan, Colors.Green, Colors.Brown, Colors.Magenta };
        public Group(int index) { Index = index; }

        public bool Contains(int motorIndex) => VM.Motors.Any(m => m.Group == Index);
        public double Position => 32.1F;
        public double Load => 2214F;


        bool isOK;
        public bool IsOK {
            get => isOK; 
            set { isOK = value; OnPropertyChanged(); }
        }

        string name;
        public string Name {
            get {
                if (name == null) return $"Group {Index + 1}";
                return name;
            }
            set { name = value; OnPropertyChanged(); }
        }

        int presetTouches;
        public int PresetTouches {
            get => presetTouches;
            set {
                presetTouches = value;
                OnPropertyChanged();
            }
        }
        public void PresetTouch() => ++PresetTouches;
        public int Index { get; set; }
        public Color Color => GroupColors[Index];
    }

    public partial class Group : IConvertible {
        public TypeCode GetTypeCode() {
            throw new NotImplementedException();
        }

        public bool ToBoolean(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider) {
            return Index;
        }

        public long ToInt64(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public object ToType(Type conversionType, IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider) {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider) {
            throw new NotImplementedException();
        }
    }
}
