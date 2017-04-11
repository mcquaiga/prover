using System.Collections.Generic;
using System.Linq;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell.Domain.Instrument;
using Prover.Domain.Instrument.Items;

namespace Prover.CommProtocol.MiHoneywell.Domain.Items
{
    internal class SuperFactorEvcItems : ISuperFactorItems
    {
        private readonly IEnumerable<ItemValue> _itemValues;

        internal SuperFactorEvcItems(IEnumerable<ItemValue> itemValues)
        {
            _itemValues = itemValues.Where(i => i.Metadata.IsSuperFactor == true);
        }

        internal SuperFactorEvcItems(HoneywellInstrument instrument) : this(instrument.ItemValues)
        {
        }

        public double Co2 => _itemValues.GetItem(55).NumericValue;
        public double N2 => _itemValues.GetItem(54).NumericValue;

        public double SpecGr => _itemValues.GetItem(53).NumericValue;

        public Dictionary<string, string> ItemData => _itemValues.ToDictionary(k => k.Metadata.Number.ToString(), v => v.RawValue);
    }
}