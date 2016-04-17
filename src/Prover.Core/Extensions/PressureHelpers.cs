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

        public static TransducerType GetTransducerType(this Instrument instrument)
        {
            return (TransducerType)instrument.Items.GetItem(TRANSDUCER_TYPE).GetNumericValue();
        }

        public static string Units(this Instrument instrument)
        {
            return instrument.Items.GetItem(PRESS_UNITS).GetDescriptionValue();
        }

        public static decimal? EvcBasePressure(this Instrument instrument)
        {
            return instrument.Items.GetItem(BASE_PRESS).GetNumericValue();
        }

        public static decimal? EvcAtmosphericPressure(this Instrument instrument)
        {
            return instrument.Items.GetItem(ATM_PRESS).GetNumericValue();
        }

        public static decimal? EvcPressureRange(this Instrument instrument)
        {
            return instrument.Items.GetItem(PRESS_RANGE).GetNumericValue();
        }

        public static decimal? EvcGasPressure(this InstrumentItems items)
        {
            return items.GetItem(GAS_PRESSURE).GetNumericValue();
        }

        public static decimal? EvcPressureFactor(this InstrumentItems items)
        {
            return items.GetItem(PRESSURE_FACTOR).GetNumericValue();
        }

        public static decimal? EvcUnsqrFactor(this Instrument instrument)
        {
            return instrument.Items.GetItem(UNSQR_FACTOR).GetNumericValue();
        }
    }
}
