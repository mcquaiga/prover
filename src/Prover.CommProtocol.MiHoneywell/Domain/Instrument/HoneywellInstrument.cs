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

        private HoneywellInstrument(int id, int accessCode, string name, string itemFilePath)
        {
            Id = id;
            AccessCode = accessCode;
            Name = name;
            ItemFilePath = itemFilePath;

            ItemDefinitions = HoneywellItemDefinitions.Load(this);
        }

        internal HoneywellInstrument(int id, int accessCode, string name, string itemFilePath, Dictionary<string, string> itemData)
            : this(id, accessCode, name, itemFilePath)
        {
            ItemValues = SetupItemValues(ItemDefinitions, itemData);
        }

        internal HoneywellInstrument(HoneywellClient commClient, int id, int accessCode, string name, string itemFilePath)
            : this(id, accessCode, name, itemFilePath)
        {
            CommClient = commClient;
        }

        public int AccessCode { get; protected set; }

        public EvcCorrectorType CorrectorType
        {
            get
            {
                var live = "live";
                var pressureLive = ItemValues.GetItem(109)?.Description.ToLower() == live;
                var tempLive = ItemValues.GetItem(111)?.Description.ToLower() == live;
                var superLive = ItemValues.GetItem(110)?.Description.ToLower() == live;

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

        public IInstrumentFactory InstrumentFactory { get; set; }

        public bool IsReadOnly => CommClient == null;

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

        public async Task GetAllItems()
        {
            ItemValues = await DownloadItemValues();
        }

        public ItemValue GetItemValue(int itemNumber)
        {
            var item = GetItemValue(itemNumber, ItemValues);
            return item;
        }

        public virtual async Task<IPressureItems> GetPressureItems()
        {
            var items = await DownloadItemValues(ItemDefinitions.PressureItems());
            return new PressureEvcItems(items);
        }

        public virtual IPressureItems GetPressureItems(Dictionary<string, string> itemData)
        {
            var itemValues = SetupItemValues(this.ItemDefinitions, itemData);
            return new PressureEvcItems(itemValues);
        }

        public virtual async Task<ITemperatureItems> GetTemperatureItems()
        {
            var items = await DownloadItemValues(ItemDefinitions.TemperatureItems());
            return new TemperatureEvcItems(items);
        }

        public virtual ITemperatureItems GetTemperatureItems(Dictionary<string, string> itemData)
        {
            var itemValues = SetupItemValues(this.ItemDefinitions, itemData);
            return new TemperatureEvcItems(itemValues);
        }

        public virtual async Task<IVolumeItems> GetVolumeItems()
        {
            var items = await DownloadItemValues(ItemDefinitions.VolumeItems());
            return new VolumeEvcItems(items);
        }

        public virtual IVolumeItems GetVolumeItems(Dictionary<string, string> itemData)
        {
            var itemValues = SetupItemValues(this.ItemDefinitions, itemData);
            return new VolumeEvcItems(itemValues);
        }

        protected async Task<IEnumerable<ItemValue>> DownloadItemValues(IEnumerable<ItemMetadata> items = null)
        {
            if (IsReadOnly) return new List<ItemValue>();

            var itemMetadatas = items?.ToList();
            if (itemMetadatas == null || !itemMetadatas.Any())
                itemMetadatas = ItemDefinitions.ToList();

            await CommClient.Connect(this, CancellationToken.None);
            var itemValues = await CommClient.GetItemValues(itemMetadatas);
            await CommClient.Disconnect();

            return itemValues;
        }

        protected ItemValue GetItemValue(int itemNumber, IEnumerable<ItemValue> itemValues)
        {
            var itemInfo = ItemDefinitions.FirstOrDefault(i => i.Number == itemNumber);
            if (itemInfo == null) return null;

            var item = itemValues.ToList().FirstOrDefault(i => i.Metadata.Number == itemNumber);

            if (item != null)
                return item;

            return new ItemValue(itemInfo, string.Empty);
        }

        protected IEnumerable<ItemValue> SetupItemValues(IEnumerable<ItemMetadata> itemsMetadata, Dictionary<string, string> itemData)
        {
            var result = new List<ItemValue>();
            var metadataList = itemsMetadata.ToList();

            foreach (var itemKeyValue in itemData)
            {
                int itemNumber;
                if (int.TryParse(itemKeyValue.Key, out itemNumber))
                {
                    var metadata = metadataList.FirstOrDefault(x => x.Number == itemNumber);
                    if (metadata != null)
                        result.Add(new ItemValue(metadata, itemKeyValue.Value));
                }
            }

            return result;
        } 
    }
}