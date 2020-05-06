using System;
using System.Text.RegularExpressions;

namespace Choreo {
    public static class FeetInchesConvert {
        /*
         * 1m = 3.28' = 3'3.37''
            const double m2f = 3.2808398950;
         */

        const double f2i = 12.0;
        public static (int feet, double inches) FeetInches(double feet) {
            var abs = Math.Abs(feet);
            var sgn = Math.Sign(feet);
            var intFeet = (int)Math.Floor(abs);
            var inches = (abs - intFeet) * f2i;
            return (sgn * intFeet, inches);
        }
        public static double Feet(int feet, double inches) => feet + inches / f2i;
        public static string ToString(double feet) {
            (double intFeet, double inches) = FeetInches(feet);
            return $"{intFeet}'-{inches:0.000}\"";
        }
        public static bool TryParse(string s, out double? feet) {
            feet = null;
            int? intFeet = null;
            double? inches = null;

            //var match = Regex.Match(s, "^([+-])?((\\d+)(')\\s?)?((\\d*\\.?\\d*)(\")?)?$");
            var match = Regex.Match(s, "^([+-])?((\\d+)(')-)?((\\d*\\.?\\d*)(\")?)?$");
            if (!match.Success) return false;
            var groups = match.Groups;
            if (groups.Count != 8) return false;

            if (groups[4].Value == "'") {
                if (!int.TryParse(groups[3].Value, out var _feet)) return false;
                intFeet = _feet;
            }

            if (!string.IsNullOrEmpty(groups[6].Value)) {
                if (!double.TryParse(groups[6].Value, out var _inches)) return false;
                if (_inches >= 12.0) return false;
                inches = Math.Round(_inches, 3);
            }

            if (intFeet == null && inches == null) return false;

            feet = Feet(intFeet??0, inches??0.0);
            if (groups[1].Value == "-") feet = -feet;
            return true;
        }
    }
}
