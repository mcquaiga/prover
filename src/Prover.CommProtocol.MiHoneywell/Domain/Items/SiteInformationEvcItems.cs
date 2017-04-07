using Prover.CommProtocol.MiHoneywell.Domain.Instrument;
using Prover.Domain.Instrument.Items;

namespace Prover.CommProtocol.MiHoneywell.Domain.Items
{
    internal class SiteInformationEvcItems : ISiteInformationItems
    {
        private const int FirmwareVersionItemNumber = 122;
        private const int SerialNumberItemNumber = 62;
        private const int SiteId1ItemNumber = 200;
        private const int SiteId2ItemNumber = 201;

        private readonly HoneywellInstrument _instrument;

        public SiteInformationEvcItems(HoneywellInstrument instrument)
        {
            _instrument = instrument;
        }

        public string FirmwareVersion => _instrument.GetItemValue(FirmwareVersionItemNumber).Description;

        public string SerialNumber => _instrument.GetItemValue(SerialNumberItemNumber).Description;
        public string SiteId1 => _instrument.GetItemValue(SiteId1ItemNumber).Description;
        public string SiteId2 => _instrument.GetItemValue(SiteId2ItemNumber).Description;
    }
}