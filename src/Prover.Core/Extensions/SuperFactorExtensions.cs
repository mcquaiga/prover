using Prover.Core.Models.Instruments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Prover.Core.Models.Instruments.SuperFactorTest;

namespace Prover.Core.Extensions
{
    public static class SuperFactorExtensions
    {
        private const int SPEC_GR_NUMBER = 53;
        private const int N2_NUMBER = 54;
        private const int CO2_NUMBER = 55;
        private const int SUPER_TABLE_NUMBER = 147;

        public static decimal? SpecGr(this Instrument instrument) => instrument.ItemDetails.GetItem(SPEC_GR_NUMBER).GetNumericValue(instrument.ItemValues);
        public static decimal? CO2(this Instrument instrument) => instrument.ItemDetails.GetItem(CO2_NUMBER).GetNumericValue(instrument.ItemValues);
        public static decimal? N2(this Instrument instrument) => instrument.ItemDetails.GetItem(N2_NUMBER).GetNumericValue(instrument.ItemValues);
        public static SuperFactorTable SuperTable(this Instrument instrument) => (SuperFactorTable)instrument.ItemDetails.GetItem(SUPER_TABLE_NUMBER).GetNumericValue(instrument.ItemValues);

    }
}
