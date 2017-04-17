using System;
using System.Collections.Generic;
using System.Linq;
using Prover.InstrumentProtocol.Core.Items;
using Prover.InstrumentProtocol.Core.Models.Instrument.Items;
using Prover.InstrumentProtocol.Honeywell.Domain.Instrument;
using Prover.Shared.Enums;

namespace Prover.InstrumentProtocol.Honeywell.Domain.Items
{
    internal class TemperatureEvcItems : ITemperatureItems
    {
        private const int BaseItemNumber = 34;
        private const int GasTempItemNumber = 26;
        private const int TempFactorItemNumber = 45;
        private const int UnitsItemNumber = 89;
        private readonly IEnumerable<ItemValue> _itemValues;

        internal TemperatureEvcItems(IEnumerable<ItemValue> itemValues)
        {
            _itemValues = itemValues.Where(i => i.Metadata.IsTemperature == true || i.Metadata.IsTemperatureTest == true);
        }

        internal TemperatureEvcItems(HoneywellInstrument instrument) : this(instrument.ItemValues)
        {
        }

        public double Base => _itemValues.GetItem(BaseItemNumber).NumericValue;
        public double Factor => _itemValues.GetItem(TempFactorItemNumber).NumericValue;

        public double GasTemperature => _itemValues.GetItem(GasTempItemNumber).NumericValue;

        public Dictionary<string, string> ItemData
            => _itemValues.ToDictionary(k => k.Metadata.Number.ToString(), v => v.RawValue);

        public TemperatureUnits Units
            => (TemperatureUnits) Enum.Parse(typeof(TemperatureUnits), _itemValues.GetItem(UnitsItemNumber).Description)
        ;
    }
}