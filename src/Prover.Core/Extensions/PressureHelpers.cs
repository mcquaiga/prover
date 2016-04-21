using Prover.Core.Models;
using Prover.Core.Models.Instruments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.Extensions
{
    public static class PressureItems
    {
        private const string BASE_PRESS = "BASE_PRESS";
        private const string ATM_PRESS = "ATM_PRESS";
        private const string PRESS_UNITS = "PRESS_UNITS";
        private const string PRESS_RANGE = "PRESS_RANGE";
        private const string TRANSDUCER_TYPE = "TRANSDUCER_TYPE";
        private const int GAS_PRESSURE = 8;
        private const int PRESSURE_FACTOR = 44;
        private const int UNSQR_FACTOR = 47;

        public static TransducerType GetTransducerType(this Instrument instrument) => (TransducerType)instrument.Items.GetItem(TRANSDUCER_TYPE).GetNumericValue(instrument.ItemValues);

        public static string Units(this Instrument instrument) => instrument.Items.GetItem(PRESS_UNITS).GetDescriptionValue(instrument.ItemValues);

        public static decimal? EvcBasePressure(this Instrument instrument) => instrument.Items.GetItem(BASE_PRESS).GetNumericValue(instrument.ItemValues);

        public static decimal? EvcAtmosphericPressure(this Instrument instrument) => instrument.Items.GetItem(ATM_PRESS).GetNumericValue(instrument.ItemValues);

        public static decimal? EvcPressureRange(this Instrument instrument) => instrument.Items.GetItem(PRESS_RANGE).GetNumericValue(instrument.ItemValues);

        public static decimal? EvcGasPressure(this Dictionary<int, string> itemValues)
        {
            return itemValues.GetItemValue(GAS_PRESSURE);
        }

        public static decimal? EvcPressureFactor(this Dictionary<int, string> itemValues)
        {
            return itemValues.GetItemValue(PRESSURE_FACTOR);
        }

        public static decimal? EvcUnsqrFactor(this Dictionary<int, string> itemValues)
        {
            return itemValues.GetItemValue(UNSQR_FACTOR);
        }
    }
}
