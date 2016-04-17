using Prover.Core.Models;
using Prover.Core.Models.Instruments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.Extensions
{
    public static class TemperatureItems
    {
        const int GAS_TEMP = 26;
        const int TEMP_FACTOR = 45;
        const int TEMP_UNITS = 89;
        const int BASE_TEMP = 34;

        public static string Range(this Instrument instrument) => "-40 - 150 " + instrument.TemperatureUnits();

        public static string TemperatureUnits(this Instrument instrument) => instrument.Items.GetItem(TEMP_UNITS).GetDescriptionValue();

        public static decimal? EvcBaseTemperature(this Instrument instrument) => instrument.Items.GetItem(BASE_TEMP).GetNumericValue();

        public static decimal? EvcTemperatureReading(this InstrumentItems items)
        {
            return items.GetItem(GAS_TEMP).GetNumericValue();
        }

        public static decimal? EvcTemperatureFactor(this InstrumentItems items)
        {
            return items.GetItem(TEMP_FACTOR).GetNumericValue();
        }

        public static bool HasPassed(this TemperatureTest test)
        {
            return (test.PercentError < 1 && test.PercentError > -1);
        }
    }
}
