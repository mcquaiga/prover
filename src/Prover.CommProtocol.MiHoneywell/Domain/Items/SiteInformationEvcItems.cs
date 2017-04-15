using System.Collections.Generic;
using System.Linq;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.Common.Models.Instrument.Items;
using Prover.CommProtocol.MiHoneywell.Domain.Instrument;

namespace Prover.CommProtocol.MiHoneywell.Domain.Items
{
    internal class SiteInformationEvcItems : ISiteInformationItems
    {
        private readonly IEnumerable<ItemValue> _itemValues;
        private const int FirmwareVersionItemNumber = 122;
        private const int SerialNumberItemNumber = 62;
        private const int SiteId1ItemNumber = 200;
        private const int SiteId2ItemNumber = 201;

        public SiteInformationEvcItems(IEnumerable<ItemValue> itemValues)
        {
            _itemValues = itemValues;
        }

        public SiteInformationEvcItems(HoneywellInstrument instrument) : this(instrument.ItemValues)
        {
        }

        public string FirmwareVersion => _itemValues.GetItem(FirmwareVersionItemNumber).Description;
        public string SerialNumber => _itemValues.GetItem(SerialNumberItemNumber).Description;
        public string SiteId1 => _itemValues.GetItem(SiteId1ItemNumber).Description;
        public string SiteId2 => _itemValues.GetItem(SiteId2ItemNumber).Description;

        public Dictionary<string, string> ItemData
            => _itemValues.ToList().ToDictionary(k => k.NumericValue.ToString(), v => v.RawValue);
    }
}