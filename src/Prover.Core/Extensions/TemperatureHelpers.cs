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

        public static string TemperatureUnits(this Instrument instrument) => instrument.ItemDetails.GetItem(TEMP_UNITS).GetDescriptionValue(instrument.ItemValues);

        public static decimal? EvcBaseTemperature(this Instrument instrument) => instrument.ItemDetails.GetItem(BASE_TEMP).GetNumericValue(instrument.ItemValues);

        public static decimal? EvcTemperatureReading(this Dictionary<int, string> itemValues)
        {
            return itemValues.GetItemValue(GAS_TEMP);
        }

        public static decimal? EvcTemperatureFactor(this Dictionary<int, string> itemValues)
        {
            return itemValues.GetItemValue(TEMP_FACTOR);
        }
    }
}
