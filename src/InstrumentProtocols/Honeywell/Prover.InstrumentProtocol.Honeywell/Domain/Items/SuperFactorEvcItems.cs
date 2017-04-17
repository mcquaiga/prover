using System;
using System.Collections.Generic;
using System.Linq;
using Prover.InstrumentProtocol.Core.Items;
using Prover.InstrumentProtocol.Core.Models.Instrument.Items;
using Prover.InstrumentProtocol.Honeywell.Domain.Instrument;

namespace Prover.InstrumentProtocol.Honeywell.Domain.Items
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

        public double Co2
        {
            get { return _itemValues.GetItem(55).NumericValue; }
            set { throw new NotImplementedException(); }
        }

        public Dictionary<string, string> ItemData
            => _itemValues.ToDictionary(k => k.Metadata.Number.ToString(), v => v.RawValue);

        public double N2
        {
            get { return _itemValues.GetItem(54).NumericValue; }
            set { throw new NotImplementedException(); }
        }

        public double SpecGr
        {
            get { return _itemValues.GetItem(53).NumericValue; }
            set { throw new NotImplementedException(); }
        }
    }
}