using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Prover.CommProtocol.Common.Instruments;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell.CommClients;

namespace Prover.CommProtocol.MiHoneywell.Instruments
{
    internal class MiniAtInstrument : HoneywellInstrument
    {
        public MiniAtInstrument(HoneywellClient commClient)
            : base(commClient, 3, 33333, "Mini-AT", "MiniATItems.xml")
        {
        }

        public override IRotaryMeterItems RotaryItems => null;

        public override IEnergyItems EnergyItems => new EnergyData(ItemValues);
        internal class EnergyData : IEnergyItems
        {
            private readonly IEnumerable<ItemValue> _itemValues;

            public EnergyData(IEnumerable<ItemValue> itemValues)
            {
                _itemValues = itemValues;
            }
            public string EnergyUnits => _itemValues.GetItem(141).Description;

            public decimal EnergyGasValue => _itemValues.GetItem(142).NumericValue;
            public decimal Energy => _itemValues.GetItem(140).NumericValue;
        }

        public override async Task<IVolumeItems> DownloadVolumeItems()
        {
            var items = await CommClient.GetItemValues(ItemDefinitions.VolumeItems());
            return new Volume(items);
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

            public decimal UncorrectedReading => _itemValues.GetItem(2).NumericValue;
            public decimal UncorrectedMultiplier => _itemValues.GetItem(92).NumericValue;
            public string UncorrectedUnits => _itemValues.GetItem(92).Description;

            public decimal CorrectedReading => _itemValues.GetHighResolutionValue(0, 113);
            public decimal CorrectedMultiplier => _itemValues.GetItem(90).NumericValue;
            public string CorrectedUnits => _itemValues.GetItem(90).Description;
            public void Update(IVolumeItems volumeItems)
            {
                throw new System.NotImplementedException();
            }

            public decimal DriveRate => _itemValues.GetItem(98).NumericValue;
            public string EnergyUnits => _itemValues.GetItem(141).Description;
            public decimal Energy => _itemValues.GetItem(140).NumericValue;
            public decimal EnergyGasValue => _itemValues.GetItem(142).NumericValue;
            public decimal UnCorrectedInputVolume(decimal appliedInput)
            {
                return appliedInput * UncorrectedMultiplier;
            }
        }
    }
}