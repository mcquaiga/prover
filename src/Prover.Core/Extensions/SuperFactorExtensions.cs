using Prover.CommProtocol.Common.Items;
using Prover.Core.Models.Instruments;

namespace Prover.Core.Extensions
{
    public static class SuperFactorExtensions
    {
        private const int SPEC_GR_NUMBER = 53;
        private const int N2_NUMBER = 54;
        private const int CO2_NUMBER = 55;
        private const int SUPER_TABLE_NUMBER = 147;

        public static decimal? SpecGr(this Instrument instrument)
        {
            return instrument.Items.GetItem(SPEC_GR_NUMBER).NumericValue;
        }

        public static decimal? CO2(this Instrument instrument)
        {
            return instrument.Items.GetItem(CO2_NUMBER).NumericValue;
        }

        public static decimal? N2(this Instrument instrument)
        {
            return instrument.Items.GetItem(N2_NUMBER).NumericValue;
        }
    }
}