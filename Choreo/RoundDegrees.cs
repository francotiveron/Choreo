using System;
using System.Text.RegularExpressions;

namespace Choreo
{
    public static class RoundsDegreesConvert
    {
        const double r2d = 360.0;
        static (char sgn, int round, double degrees) RoundsDegrees(double rounds)
        {
            var abs = Math.Abs(rounds);
            var sgn = Math.Sign(rounds);
            var intRounds = (int)Math.Floor(abs);
            var degrees = Math.Min(359, Math.Round((abs - intRounds) * r2d));
            return (sgn < 0 ? '-' : '+', intRounds, degrees);
        }
        public static double Rounds(int rounds, double degrees) => rounds + degrees / r2d;
        public static string ToString(double rounds) {
            (char sgn, int intRounds, double degrees) = RoundsDegrees(rounds);
            if (sgn == '+') return $" {intRounds}'{degrees:000}\"";
            else return $"{sgn}{intRounds}'{degrees:000}\"";
        }
        public static bool TryParse(string s, out double? rounds) {
            rounds = null;
            int? intRounds = null;
            double? degrees = null;

            s = s.Replace(' ', '+');

            var match = Regex.Match(s, "^([+-])?((\\d+)('))?((\\d*\\.?\\d*)(\")?)?$");
            if (!match.Success) return false;
            var groups = match.Groups;
            if (groups.Count != 8) return false;

            if (groups[4].Value == "'") {
                if (!int.TryParse(groups[3].Value, out var _rounds)) return false;
                intRounds = _rounds;
            }

            if (!string.IsNullOrEmpty(groups[6].Value)) {
                if (!double.TryParse(groups[6].Value, out var _degrees)) return false;
                if (_degrees >= r2d) return false;
                degrees = Math.Round(_degrees, 3);
            }

            if (intRounds == null && degrees == null) return false;

            rounds = Rounds(intRounds??0, degrees??0.0);
            if (groups[1].Value == "-") rounds = -rounds;
            return true;
        }
    }
}
