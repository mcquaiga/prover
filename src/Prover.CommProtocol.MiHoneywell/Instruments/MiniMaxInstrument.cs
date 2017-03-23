using System.Collections.Generic;
using System.Threading.Tasks;
using Prover.CommProtocol.Common.Instruments;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell.CommClients;

namespace Prover.CommProtocol.MiHoneywell.Instruments
{
    internal class MiniMaxInstrument : HoneywellInstrument
    {
        public MiniMaxInstrument(HoneywellClient commClient)
            : base(commClient, 4, 33333, "Mini-Max", "MiniMaxItems.xml")
        {
        }

        public override IVolumeItems VolumeItems => new Volume(this);

        internal class Volume : IVolumeItems
        {
            private readonly IEnumerable<ItemValue> _itemValues;

            public Volume(IEnumerable<ItemValue> itemValues)
            {
                _itemValues = itemValues;
            }

            public Volume(HoneywellInstrument instrument) : this(instrument.ItemValues) { }

            public decimal UncorrectedReading => _itemValues.GetHighResolutionValue(2, 892);
            public decimal UncorrectedMultiplier => _itemValues.GetItem(92).NumericValue;
            public string UncorrectedUnits => _itemValues.GetItem(92).Description;

            public decimal CorrectedReading => _itemValues.GetHighResolutionValue(0, 113);
            public decimal CorrectedMultiplier => _itemValues.GetItem(90).NumericValue;
            public string CorrectedUnits => _itemValues.GetItem(90).Description;
            public void Update(IVolumeItems volumeItems)
            {
                throw new System.NotImplementedException();
            }

            public decimal DriveRate { get; }
            public decimal UnCorrectedInputVolume(decimal appliedInput)
            {
                return appliedInput;
            }
        }

        public override IRotaryMeterItems RotaryItems => new Rotary(this.ItemValues);

        internal class Rotary : IRotaryMeterItems
        {
            private readonly IEnumerable<ItemValue> _itemValues;
            
            public Rotary(IEnumerable<ItemValue> itemValues)
            {
                _itemValues = itemValues;
            }

            public string MeterModel => _itemValues.GetItem(432).Description;
            public decimal MeterDisplacement => _itemValues.GetItem(439).NumericValue;
        }

        public override async Task<IVolumeItems> DownloadVolumeItems()
        {
            var items = await CommClient.GetItemValues(ItemDefinitions.VolumeItems());
            return new Volume(items);
        }

        public override IEnergyItems EnergyItems => null;
    }
}