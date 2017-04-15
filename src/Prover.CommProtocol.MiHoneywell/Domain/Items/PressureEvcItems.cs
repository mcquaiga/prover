using System;
using System.Collections.Generic;
using System.Linq;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.Common.Models.Instrument.Items;
using Prover.CommProtocol.MiHoneywell.Domain.Instrument;
using Prover.Shared.Enums;

namespace Prover.CommProtocol.MiHoneywell.Domain.Items
{
    internal class PressureEvcItems : IPressureItems
    {
        private const int AtmPressureItemNumber = 14;
        private const int BaseItemNumber = 13;
        private const int FactorItemNumber = 44;
        private const int GasPressureItemNumber = 8;
        private const int RangeItemNumber = 137;
        private const int TransducerTypeItemNumber = 112;
        private const int UnitsItemNumber = 87;
        private const int UnsqrFactorItemNumber = 47;

        private readonly Dictionary<int, string> _items = new Dictionary<int, string>
        {
            {UnitsItemNumber, "Pressure Units"},
            {GasPressureItemNumber, "Gas Pressure"},
            {AtmPressureItemNumber, "ATM Pressure"},
            {BaseItemNumber, "Base Pressure"},
            {FactorItemNumber, "Pressure Factor"},
            {RangeItemNumber, "Pressure Range"},
            {TransducerTypeItemNumber, "Transducer Type"},
            {UnsqrFactorItemNumber, "Unsqr. Factor"}
        };

        private readonly IEnumerable<ItemValue> _itemValues;

        public PressureEvcItems(IEnumerable<ItemValue> itemValues)
        {
            _itemValues =
                itemValues.Where(i => i.Metadata.IsPressure == true || i.Metadata.IsPressureTest == true).ToList();

            if (!ValidateItemList(_items, _itemValues))
                throw new Exception("Pressure item values do not contain all the correct item numbers");

            Units = (PressureUnits) _itemValues.GetItem(UnitsItemNumber).NumericValue;
            Range = (int) _itemValues.GetItem(RangeItemNumber).NumericValue;

            TransducerType = (PressureTransducerType)Enum.Parse(typeof(PressureTransducerType), _itemValues.GetItem(TransducerTypeItemNumber).Description);

            Base = _itemValues.GetItem(BaseItemNumber).NumericValue;
            GasPressure = _itemValues.GetItem(GasPressureItemNumber).NumericValue;
            AtmosphericPressure = _itemValues.GetItem(AtmPressureItemNumber).NumericValue;
            Factor = _itemValues.GetItem(FactorItemNumber).NumericValue;
            UnsqrFactor = _itemValues.GetItem(UnsqrFactorItemNumber).NumericValue;
        }

        public PressureEvcItems(HoneywellInstrument instrument) : this(instrument.ItemValues)
        {
        }

        public double AtmosphericPressure { get; set; }
        public double Base { get; set; }
        public double Factor { get; set; }
        public double GasPressure { get; set; }

        public Dictionary<string, string> ItemData
            => _itemValues.ToDictionary(k => k.Metadata.Number.ToString(), v => v.RawValue);

        public int Range { get; set; }
        public PressureTransducerType TransducerType { get; set; }

        public PressureUnits Units { get; set; }
        public double UnsqrFactor { get; set; }

        private bool ValidateItemList(Dictionary<int, string> itemNumbersToValidate, IEnumerable<ItemValue> itemValues)
        {
            foreach (var item in itemNumbersToValidate)
                if (itemValues.GetItem(item.Key) == null)
                    return false;

            return true;
        }
    }
}