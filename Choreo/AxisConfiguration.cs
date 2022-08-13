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
                if (axis.IsMotor) RotationsPerFoot = axis.RotationsPerFoot; else RotationsPerFoot = 0;
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
                JogAcc = axis.JogAcc;
                JogDec = axis.JogDec;
                if (axis is Motor m)
                {
                    Jerk = m.Jerk;
                    PGain = m.PGain;
                    RefVel = m.RefVel;
                }
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
        [Persistent]
        public double JogAcc { get; set; }
        [Persistent]
        public double JogDec { get; set; }
        [Persistent]
        public double Jerk { get; set; }
        [Persistent]
        public double PGain { get; set; }
        [Persistent]
        public double RefVel { get; set; }


        public void Download(Axis axis)
        {
            axis.CalibrationValue = CalibrationValue;
            if (axis.IsMotor) axis.RotationsPerFoot = RotationsPerFoot; else axis.RotationsPerFoot = 1;
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
            axis.JogAcc = JogAcc;
            axis.JogDec = JogDec;

            if (axis is Motor m)
            {
                m.Jerk = Jerk;
                m.PGain = PGain;
                m.RefVel = RefVel;
            }
        }

        public override string ToString() => Name;
    }
}
