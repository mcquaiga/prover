using System.Collections.Generic;
using Prover.InstrumentProtocol.Core.Items;
using Prover.InstrumentProtocol.Honeywell.Domain.Items;

namespace Prover.InstrumentProtocol.Honeywell.Domain.Instrument.MiniMax
{
    internal class MiniMaxVolume : VolumeEvcItems
    {
        public MiniMaxVolume(HoneywellInstrument instrument) : base(instrument.ItemValues)
        {
        }

        internal MiniMaxVolume(IEnumerable<ItemValue> itemValues) : base(itemValues)
        {
        }

        public override double CorrectedReading => ItemValues.GetHighResolutionValue(0, 113);

        public override double MeterDisplacement => ItemValues.GetItem(439).NumericValue;
        public override string MeterModel => ItemValues.GetItem(432).Description;
        public override int MeterModelId => (int) ItemValues.GetItem(432).NumericValue;

        public override double UncorrectedReading => ItemValues.GetHighResolutionValue(2, 892);
    }
}