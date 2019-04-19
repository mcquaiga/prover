using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.Common.Models.Instrument;
using Prover.CommProtocol.Common.Models.Instrument.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.CommProtocol.MiHoneywell.Devices
{
    public class HoneywellEvcDevice : IEvcDevice
    {
        public int AccessCode { get; set; }

        public EvcCommunicationClient CommClient => throw new NotImplementedException();

        public EvcCorrectorType CorrectorType => throw new NotImplementedException();

        public int Id { get; set; }

        public IEnumerable<ItemMetadata> ItemDefinitions => throw new NotImplementedException();

        public string ItemFilePath { get; }
        public string Name { get; set; }

        public IPressureItems PressureItems => throw new NotImplementedException();

        public ISiteInformationItems SiteInformationItems => throw new NotImplementedException();

        public ISuperFactorItems SuperFactorItems => throw new NotImplementedException();

        public ITemperatureItems TemperatureItems => throw new NotImplementedException();

        public IVolumeItems VolumeItems => throw new NotImplementedException();

        public Task GetAllItems()
        {
            throw new NotImplementedException();
        }

        public Task<IPressureItems> GetPressureItems()
        {
            throw new NotImplementedException();
        }

        public IPressureItems GetPressureItems(Dictionary<string, string> itemData)
        {
            throw new NotImplementedException();
        }

        public Task<ITemperatureItems> GetTemperatureItems()
        {
            throw new NotImplementedException();
        }

        public ITemperatureItems GetTemperatureItems(Dictionary<string, string> itemData)
        {
            throw new NotImplementedException();
        }

        public Task<IVolumeItems> GetVolumeItems()
        {
            throw new NotImplementedException();
        }

        public IVolumeItems GetVolumeItems(Dictionary<string, string> itemData)
        {
            throw new NotImplementedException();
        }
    }
}
