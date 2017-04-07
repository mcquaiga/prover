using System.Collections.Generic;
using System.Linq;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell.Domain.Instrument;
using Prover.Domain.Instrument.Items;

namespace Prover.CommProtocol.MiHoneywell.Domain.Items
{
    internal class PressureEvcItems : IPressureItems
    {
        private readonly IEnumerable<ItemValue> _itemValues;

        internal PressureEvcItems(IEnumerable<ItemValue> itemValues)
        {
            _itemValues =
                itemValues.Where(i => i.Metadata.IsPressure == true || i.Metadata.IsPressureTest == true).ToList();

            Range = (int) _itemValues.GetItem(137).NumericValue;
            TransducerType = _itemValues.GetItem(112).Description;
            Base = _itemValues.GetItem(13).NumericValue;
            GasPressure = _itemValues.GetItem(8).NumericValue;
            AtmPressure = _itemValues.GetItem(14).NumericValue;
            Factor = _itemValues.GetItem(44).NumericValue;
            UnsqrFactor = _itemValues.GetItem(47).NumericValue;
        }

        internal PressureEvcItems(HoneywellInstrument instrument) : this(instrument.ItemValues)
        {
        }

        public decimal AtmPressure { get; set; }
        public decimal Base { get; set; }
        public decimal Factor { get; set; }
        public decimal GasPressure { get; set; }

        public Dictionary<string, string> ItemData =>
            _itemValues.ToDictionary(k => k.Metadata.Number.ToString(), v => v.RawValue);

        public int Range { get; set; }
        public string TransducerType { get; set; }
        public decimal UnsqrFactor { get; set; }
    }
}