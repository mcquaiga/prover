using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.SerialProtocol;
using JsonConfig;

namespace Prover.Core.Settings
{
    public static class Instrument
    {
        public static string CommPortName
        {
            get
            {
                if (Config.Global.Instrument.CommPortName != null)
                    return Config.Global.Instrument.CommPortName;
                return string.Empty;
            }

            set
            {
                Config.Global.Instrument.CommPort = value;
            }
        }

        public static BaudRateEnum BaudRate
        {
            get
            {
                if (Config.Global.Instrument.BaudRate != null)
                    return (BaudRateEnum)Enum.Parse(typeof(BaudRateEnum), Config.Global.Instrument.BaudRate.ToString());
                return BaudRateEnum.b38400;            }
            set
            {
                Config.Global.Instrument.BaudRate = value.ToString();
            }
        }
    }

    public static class Tachometer
    {
        public static string CommPortName
        {
            get
            {
                if (Config.Global.Tachometer.CommPortName != null)
                    return Config.Global.Tachometer.CommPortName;
                return string.Empty;
            }

            set
            {
                Config.Global.TachometerSettings.CommPort = value;
            }
        }
    }
}
