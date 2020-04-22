using System;
using System.Text.RegularExpressions;

namespace Choreo {
    public class FeetInchesConvert {
        /*
         * 1m = 3.28' = 3'3.37''
         */
        const double m2f = 3.2808398950;
        const double f2i = 12.0;
        double r2f = 1.0;

        public FeetInchesConvert(double feetPerRotation) { r2f = feetPerRotation; }

        public double Feet(double rotations) => rotations * r2f;
        public double Rotations(double feet) => feet / r2f;
        public (double feet, double inches) FeetInches(double rotations) {
            var feet = Feet(rotations);
            var intFeet = (int)Math.Floor(feet);
            var inches = (feet - intFeet) * f2i;
            return (intFeet, inches);
        }
        public double Rotations(double feet, double inches) {
            var totfeet = feet + inches / f2i;
            return Rotations(totfeet);
        }
        public string ToString(double rotations) {
            (double feet, double inches) = FeetInches(rotations);
            return $"{feet}'{inches:0.0}''";
        }
        public bool TryParse(string s, out double? rotations) {
            rotations = null;
            double? feet = null, inches = null;

            var match = Regex.Match(s, "^([+-])?((\\d+)('))?((\\d*\\.?\\d*)(\"))?$");
            if (!match.Success) return false;
            var groups = match.Groups;
            if (groups.Count != 8) return false;

            if (groups[4].Value == "'") {
                if (!int.TryParse(groups[3].Value, out var _feet)) return false;
                feet = _feet;
            }

            if (groups[7].Value == "\"") {
                if (!double.TryParse(groups[6].Value, out var _inches)) return false;
                if (_inches >= 12.0) return false;
                inches = _inches;
            }

            if (feet == null && inches == null) return false;

            rotations = Rotations(feet??0.0, inches??0.0);
            if (groups[1].Value == "-") rotations = -rotations;
            return true;
        }
    }
}
