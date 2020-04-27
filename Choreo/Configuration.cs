using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;

namespace Choreo
{
    static class Configuration
    {
        //public static AmsNetId AmsNetId {
        //    get {
        //        try {
        //            return AmsNetId.Parse(ConfigurationManager.AppSettings[nameof(AmsNetId)]);
        //        }
        //        catch {
        //            return Properties.PlcSettings.Default.AmsNetId;
        //        }
        //    }

        //}
        //public static AmsPort AmsPort => Properties.PlcSettings.Default.AmsPort;

        public static AmsNetId PlcAmsNetId => Parse(AmsNetId.Parse);
        public static AmsPort PlcAmsPort => Parse((s) => (AmsPort)Enum.Parse(typeof(AmsPort), s));

        static T Parse<T>(Func<string, T> parser) {
            string typeName = typeof(T).Name;
            var @default = Properties.Settings.Default;

            try {
                return parser(ConfigurationManager.AppSettings[typeName]);
            }
            catch {
                var pi = @default.GetType().GetProperty(typeName);
                return (T)pi.GetValue(@default);
            }
        }
    }
}
