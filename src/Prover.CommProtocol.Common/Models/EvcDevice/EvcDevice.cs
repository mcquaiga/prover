using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.Common.Models.Instrument.Items;
using Prover.Core.Shared.Enums;

namespace Prover.CommProtocol.Common.Models.Instrument
{
    internal abstract class EvcDevice : IEvcDevice
    {
        public int AccessCode { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public string ItemFilePath { get; set; }

        public EvcCommunicationClient CommClient {get; }

        public EvcCorrectorType CorrectorType { get; }

        public IEnumerable<ItemMetadata> ItemDefinitions { get; }

        protected Dictionary<int, string> ItemData { get; }

        public IPressureItems PressureItems => throw new NotImplementedException();

        public ISiteInformationItems SiteInformationItems => throw new NotImplementedException();

        public ISuperFactorItems SuperFactorItems => throw new NotImplementedException();

        public ITemperatureItems TemperatureItems => throw new NotImplementedException();

        public IVolumeItems VolumeItems => throw new NotImplementedException();

        public virtual async Task GetAllItems()
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
