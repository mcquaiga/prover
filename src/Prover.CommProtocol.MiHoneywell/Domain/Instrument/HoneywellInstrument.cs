using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell.CommClients;
using Prover.CommProtocol.MiHoneywell.Domain.Items;
using Prover.Domain.Instrument;
using Prover.Domain.Instrument.Items;
using Prover.Shared.Enums;

namespace Prover.CommProtocol.MiHoneywell.Domain.Instrument
{
    internal class HoneywellInstrument : IInstrument
    {
        protected readonly HoneywellClient CommClient;

        public HoneywellInstrument(int id, int accessCode, string name, string itemFilePath)
        {
            Id = id;
            AccessCode = accessCode;
            Name = name;
            ItemFilePath = itemFilePath;

            ItemDefinitions = HoneywellItemDefinitions.Load(this);
        }

        public HoneywellInstrument(HoneywellClient commClient, int id, int accessCode, string name,
            string itemFilePath)
            : this(id, accessCode, name, itemFilePath)
        {
            CommClient = commClient;
        }

        public int AccessCode { get; protected set; }

        public EvcCorrectorType CorrectorType
        {
            get
            {
                var live = "Live";
                var pressureLive = GetItemValue(109).Description == live;
                var tempLive = GetItemValue(111).Description == live;
                var superLive = GetItemValue(110).Description == live;

                if (pressureLive && tempLive && superLive)
                    return EvcCorrectorType.PTZ;

                if (pressureLive)
                    return EvcCorrectorType.P;

                if (tempLive)
                    return EvcCorrectorType.T;

                throw new NotSupportedException("Could not determine the corrector type.");
            }
        }

        public int Id { get; protected set; }

        public Dictionary<string, string> ItemData
            => ItemValues.ToDictionary(k => k.Metadata.Number.ToString(), v => v.RawValue);

        public string ItemFilePath { get; protected set; }
        public IEnumerable<ItemValue> ItemValues { get; protected set; }
        public string Name { get; protected set; }

        public virtual IPressureItems PressureItems
            => new PressureEvcItems(this);

        public virtual ISiteInformationItems SiteInformationItems
            => new SiteInformationEvcItems(this);

        public virtual ISuperFactorItems SuperFactorItems
            => new SuperFactorEvcItems(this);

        public virtual ITemperatureItems TemperatureItems
            => new TemperatureEvcItems(this);

        public virtual IVolumeItems VolumeItems
            => new VolumeEvcItems(this);

        protected IEnumerable<ItemMetadata> ItemDefinitions { get; }

        public virtual async Task<IPressureItems> GetPressureItems()
        {
            var items = await GetItems(ItemDefinitions.PressureItems());
            return new PressureEvcItems(items);
        }

        public virtual async Task<ITemperatureItems> GetTemperatureItems()
        {
            var items = await GetItems(ItemDefinitions.TemperatureItems());
            return new TemperatureEvcItems(items);
        }

        public virtual async Task<IVolumeItems> GetVolumeItems()
        {
            var items = await GetItems(ItemDefinitions.VolumeItems());
            return new VolumeEvcItems(items);
        }

        internal async Task GetAllItems()
        {
            ItemValues = await GetItems();
        }

        internal ItemValue GetItemValue(int itemNumber)
        {
            var item = ItemValues.ToList().FirstOrDefault(i => i.Metadata.Number == itemNumber);
            return item;
        }

        private async Task<IEnumerable<ItemValue>> GetItems(IEnumerable<ItemMetadata> items = null)
        {
            var itemMetadatas = items?.ToList();
            if (itemMetadatas == null || !itemMetadatas.Any())
                itemMetadatas = ItemDefinitions.ToList();

            await CommClient.Connect(this, CancellationToken.None);
            var itemValues = await CommClient.GetItemValues(itemMetadatas);
            await CommClient.Disconnect();

            return itemValues;
        }

        private ItemValue GetItemValue(string itemCode, IEnumerable<ItemValue> itemValues)
        {
            if (string.IsNullOrEmpty(itemCode)) return null;

            var itemInfo = ItemDefinitions.FirstOrDefault(i => i.Code == itemCode);
            if (itemInfo == null) return null;

            var item = itemValues.ToList().FirstOrDefault(i => i.Metadata.Code == itemCode);

            if (item != null)
                return item;

            return new ItemValue(itemInfo, string.Empty);
        }
    }
}