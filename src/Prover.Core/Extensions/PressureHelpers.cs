using Prover.CommProtocol.Common.Items;
using Prover.Core.Models.Instruments;

namespace Prover.Core.Extensions
{
    public static class PressureItems
    {
        public static TransducerType GetTransducerType(this Instrument instrument)
            => (TransducerType) instrument.Items.GetItem(ItemCodes.Pressure.TransducerType).NumericValue;

        public static string PressureUnits(this Instrument instrument)
            => instrument.Items.GetItem(ItemCodes.Pressure.Units).Description;

        public static decimal? EvcBasePressure(this Instrument instrument)
            => instrument.Items.GetItem(ItemCodes.Pressure.Base).NumericValue;

        public static decimal? EvcAtmosphericPressure(this Instrument instrument)
            => instrument.Items.GetItem(ItemCodes.Pressure.Atm).NumericValue;

        public static decimal? EvcPressureRange(this Instrument instrument)
            => instrument.Items.GetItem(ItemCodes.Pressure.Range).NumericValue;

        //public static decimal? EvcPressureFactor(this Dictionary<int, string> itemValues)
        //}
        //    return itemValues.GetItem(ItemCodes.Pressure.GasPressure);
        //{

        //public static decimal? EvcGasPressure(this Dictionary<int, string> itemValues)
        //{
        //    return itemValues.GetItemValue(ItemCodes.Pressure.Factor);
        //}

        //public static decimal? EvcUnsqrFactor(this Dictionary<int, string> itemValues)
        //{
        //    return itemValues.GetItemValue(ItemCodes.Pressure.UnsqrFactor);
        //}
    }
}