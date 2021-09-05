using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Choreo
{
    public class AxisConfiguration
    {
        public AxisConfiguration(string name, Axis axis = null)
        {
            Name = name;
            if (axis != null)
            {
                CalibrationValue = axis.CalibrationValue;
                RotationsPerFoot = axis.RotationsPerFoot;
                SoftUp = axis.SoftUp;
                SoftDn = axis.SoftDn;
                MaxAcc = axis.MaxAcc;
                DefAcc = axis.DefAcc;
                MinVel = axis.MinVel;
                MaxVel = axis.MaxVel;
                DefVel = axis.DefVel;
                MaxDec = axis.MaxDec;
                DefDec = axis.DefDec;
                MinLoad = axis.MinLoad;
                MaxLoad = axis.MaxLoad;
                LoadOffs = axis.LoadOffs;
            }
        }

        [Persistent]
        public string Name { get; }
        [Persistent]
        public double CalibrationValue { get; set; }
        [Persistent]
        public double RotationsPerFoot { get; set; }
        [Persistent]
        public double SoftUp { get; set; }
        [Persistent]
        public double SoftDn { get; set; }
        [Persistent]
        public double MaxAcc { get; set; }
        [Persistent]
        public double DefAcc { get; set; }
        [Persistent]
        public double MinVel { get; set; }
        [Persistent]
        public double MaxVel { get; set; }
        [Persistent]
        public double DefVel { get; set; }
        [Persistent]
        public double MaxDec { get; set; }
        [Persistent]
        public double DefDec { get; set; }
        [Persistent]
        public double MinLoad { get; set; }
        [Persistent]
        public double MaxLoad { get; set; }
        [Persistent]
        public double LoadOffs { get; set; }

        public void Download(Axis axis)
        {
            axis.CalibrationValue = CalibrationValue;
            axis.RotationsPerFoot = RotationsPerFoot;
            axis.SoftUp = SoftUp;
            axis.SoftDn = SoftDn;
            axis.MaxAcc = MaxAcc;
            axis.DefAcc = DefAcc;
            axis.MinVel = MinVel;
            axis.MaxVel = MaxVel;
            axis.DefVel = DefVel;
            axis.MaxDec = MaxDec;
            axis.DefDec = DefDec;
            axis.MinLoad = MinLoad;
            axis.MaxLoad = MaxLoad;
            axis.LoadOffs = LoadOffs;
        }

        public override string ToString() => Name;
    }
}
