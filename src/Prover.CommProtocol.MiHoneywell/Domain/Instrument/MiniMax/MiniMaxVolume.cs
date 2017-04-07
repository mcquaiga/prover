using System.Collections.Generic;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell.Domain.Items;

namespace Prover.CommProtocol.MiHoneywell.Domain.Instrument.MiniMax
{
    internal class MiniMaxVolume : VolumeEvcItems
    {
        public MiniMaxVolume(HoneywellInstrument instrument) : base(instrument.ItemValues)
        {
        }

        internal MiniMaxVolume(IEnumerable<ItemValue> itemValues) : base(itemValues)
        {
        }

        public override decimal CorrectedReading => ItemValues.GetHighResolutionValue(0, 113);

        public override decimal MeterDisplacement => ItemValues.GetItem(439).NumericValue;
        public override string MeterModel => ItemValues.GetItem(432).Description;
        public override int MeterModelId => (int) ItemValues.GetItem(432).NumericValue;

        public override decimal UncorrectedReading => ItemValues.GetHighResolutionValue(2, 892);
    }
}