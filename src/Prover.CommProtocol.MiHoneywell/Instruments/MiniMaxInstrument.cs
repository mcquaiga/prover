using System.Collections.Generic;
using System.Threading.Tasks;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell.CommClients;
using Prover.Domain.Models.Instruments.Items;

namespace Prover.CommProtocol.MiHoneywell.Instruments
{
    internal class MiniMaxInstrument : HoneywellInstrument
    {
        public MiniMaxInstrument(HoneywellClient commClient)
            : base(commClient, 4, 33333, "Mini-Max", "MiniMaxItems.xml")
        {
        }

        public override IVolumeItems VolumeItems => new MiniMaxVolume(this);

        internal class MiniMaxVolume : VolumeEvcItems
        {     
            internal MiniMaxVolume(IEnumerable<ItemValue> itemValues) : base(itemValues) { }
            public MiniMaxVolume(HoneywellInstrument instrument) : base(instrument.ItemValues) { }

            public override decimal UncorrectedReading => ItemValues.GetHighResolutionValue(2, 892);
            public override decimal CorrectedReading => ItemValues.GetHighResolutionValue(0, 113);

            public override decimal MeterDisplacement => ItemValues.GetItem(439).NumericValue;
            public override string MeterModel => ItemValues.GetItem(432).Description;
            public override int MeterModelId => (int) ItemValues.GetItem(432).NumericValue;
        }

        public override async Task<IVolumeItems> GetVolumeItems()
        {
            var items = await CommClient.GetItemValues(ItemDefinitions.VolumeItems());
            return new MiniMaxVolume(items);
        }
    }
}