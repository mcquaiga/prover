using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell.CommClients;
using Prover.Domain.Models.Instruments;
using Prover.Domain.Models.Instruments.Items;
using Prover.Shared.Enums;

namespace Prover.CommProtocol.MiHoneywell.Instruments
{
    public enum HoneywellInstrumentType
    {
        MiniAt,
        MiniMax
    }

    public static class HoneywellInstrumentFactory
    {
        public static async Task<IInstrument> Create(CommPort commPort, HoneywellInstrumentType instrumentType)
        {
            var commClient = new HoneywellClient(commPort);

            HoneywellInstrument instrument;
            if (instrumentType == HoneywellInstrumentType.MiniMax)
                instrument = new MiniMaxInstrument(commClient);
            else
                instrument = new MiniAtInstrument(commClient);

            await instrument.GetAllItems();
            return instrument;
        }
    }

    internal abstract class HoneywellInstrument : IInstrument
    {
        protected HoneywellClient CommClient;

        protected HoneywellInstrument(HoneywellClient commClient, int id, int accessCode, string name,
            string itemFilePath)
        {
            Id = id;
            AccessCode = accessCode;
            Name = name;
            ItemFilePath = itemFilePath;

            CommClient = commClient;
            ItemDefinitions = HoneywellItemDefinitions.Load(this);
        }

        public int Id { get; protected set; }
        public string Name { get; protected set; }
        public int AccessCode { get; protected set; }
        public string ItemFilePath { get; protected set; }
        public IEnumerable<ItemValue> ItemValues { get; protected set; }
        protected IEnumerable<ItemMetadata> ItemDefinitions { get; }
        
        public Dictionary<string, string> ItemData
            => ItemValues.ToDictionary(k => k.Metadata.Number.ToString(), v => v.RawValue);

        public virtual ISiteInformationItems SiteInformationItems
            => new SiteInformationEvcItems(this);

        public virtual ITemperatureItems TemperatureItems
            => new TemperatureEvcItems(this);

        public virtual IPressureItems PressureItems
            => new PressureEvcItems(this);

        public virtual ISuperFactorItems SuperFactorItems
            => new SuperFactorEvcItems(this);

        public virtual IVolumeItems VolumeItems => new VolumeEvcItems(this);

        public async Task<ITemperatureItems> GetTemperatureItems()
        {
            var items = await GetItems(ItemDefinitions.TemperatureItems());
            return new TemperatureEvcItems(items);
        }

        public async Task<IPressureItems> GetPressureItems()
        {
            var items = await GetItems(ItemDefinitions.PressureItems());
            return new PressureEvcItems(items);
        }

        public abstract Task<IVolumeItems> GetVolumeItems();

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

        public Task<T> GetItemsByGroup<T>(T itemGroup)
        {
            throw new NotImplementedException();
        }

        internal async Task GetAllItems()
        {
            ItemValues = await GetItems();
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

        internal ItemValue GetItemValue(int itemNumber)
        {
            var item = ItemValues.ToList().FirstOrDefault(i => i.Metadata.Number == itemNumber);
            return item;
        }
    }

    internal class SiteInformationEvcItems : ISiteInformationItems
    {
        private const int SerialNumberItemNumber = 62;
        private const int SiteId1ItemNumber = 200;
        private const int SiteId2ItemNumber = 201;
        private const int FirmwareVersionItemNumber = 122;

        private readonly HoneywellInstrument _instrument;

        public SiteInformationEvcItems(HoneywellInstrument instrument)
        {
            _instrument = instrument;
        }

        public string SerialNumber => _instrument.GetItemValue(SerialNumberItemNumber).Description;
        public string FirmwareVersion => _instrument.GetItemValue(FirmwareVersionItemNumber).Description;
        public string SiteId1 => _instrument.GetItemValue(SiteId1ItemNumber).Description;
        public string SiteId2 => _instrument.GetItemValue(SiteId2ItemNumber).Description;
    }

    internal class TemperatureEvcItems : ITemperatureItems
    {
        private const int BaseItemNumber = 34;
        private const int GasTempItemNumber = 26;
        private const int UnitsItemNumber = 89;
        private const int TempFactorItemNumber = 45;
        private readonly IEnumerable<ItemValue> _itemValues;

        internal TemperatureEvcItems(IEnumerable<ItemValue> itemValues)
        {
            _itemValues = itemValues.Where(i => i.Metadata.IsTemperature == true || i.Metadata.IsTemperatureTest == true);
        }

        internal TemperatureEvcItems(HoneywellInstrument instrument) : this(instrument.ItemValues)
        {
        }

        public decimal Base => _itemValues.GetItem(BaseItemNumber).NumericValue;

        public TemperatureUnits Units
            => (TemperatureUnits) Enum.Parse(typeof(TemperatureUnits), _itemValues.GetItem(UnitsItemNumber).Description)
        ;

        public decimal GasTemperature => _itemValues.GetItem(GasTempItemNumber).NumericValue;
        public decimal Factor => _itemValues.GetItem(TempFactorItemNumber).NumericValue;
    }

    internal class PressureEvcItems : IPressureItems
    {
        private readonly IEnumerable<ItemValue> _itemValues;

        public int Range { get; set; }
        public string TransducerType { get; set; }
        public decimal Base { get; set; }
        public decimal GasPressure { get; set; }
        public decimal AtmPressure { get; set; }
        public decimal Factor { get; set; }
        public decimal UnsqrFactor { get; set; }

        public Dictionary<string, string> ItemData =>
            _itemValues.ToDictionary(k => k.Metadata.Number.ToString(), v => v.RawValue);

        internal PressureEvcItems(IEnumerable<ItemValue> itemValues)
        {
            _itemValues = itemValues.Where(i => i.Metadata.IsPressure == true || i.Metadata.IsPressureTest == true).ToList();

            Range = (int)_itemValues.GetItem(137).NumericValue;
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
    }

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

        public decimal SpecGr => _itemValues.GetItem(53).NumericValue;
        public decimal N2 => _itemValues.GetItem(54).NumericValue;
        public decimal Co2 => _itemValues.GetItem(55).NumericValue;
    }

    internal class VolumeEvcItems : IVolumeItems
    {
        protected readonly IEnumerable<ItemValue> ItemValues;

        public VolumeEvcItems(IEnumerable<ItemValue> itemValues)
        {
            ItemValues = itemValues.Where(i => i.Metadata.IsVolume == true || i.Metadata.IsVolumeTest == true);
        }

        public VolumeEvcItems(HoneywellInstrument instrument) : this(instrument.ItemValues)
        {
        }

        public virtual string EnergyUnits => ItemValues.GetItem(141).Description;
        public virtual decimal EnergyGasValue => ItemValues.GetItem(142).NumericValue;
        public virtual decimal Energy => ItemValues.GetItem(140).NumericValue;

        public virtual decimal UncorrectedReading => ItemValues.GetItem(2).NumericValue;
        public virtual decimal UncorrectedMultiplier => ItemValues.GetItem(92).NumericValue;
        public virtual string UncorrectedUnits => ItemValues.GetItem(92).Description;

        public virtual decimal CorrectedReading => ItemValues.GetHighResolutionValue(0, 113);
        public virtual decimal CorrectedMultiplier => ItemValues.GetItem(90).NumericValue;
        public virtual string CorrectedUnits => ItemValues.GetItem(90).Description;

        public virtual decimal DriveRate => ItemValues.GetItem(98).NumericValue;
        public virtual string DriveRateDescription => ItemValues.GetItem(98).Description;

        public virtual DriveTypeDescripter DriveType => DriveRateDescription == "Rotary" ? DriveTypeDescripter.Rotary : DriveTypeDescripter.Mechanical;

        public virtual int MeterModelId => 0;
        public virtual string MeterModel => string.Empty;
        public virtual decimal MeterDisplacement => 0.0m;  
    }

    internal static class HighResVolumeHelpers
    {
        public static decimal GetHighResolutionValue(this IEnumerable<ItemValue> itemValues, int lowResItemNumber,
            int highResItemNumber)
        {
            if (itemValues == null) return 0.0m;

            var items = itemValues as ItemValue[] ?? itemValues.ToArray();
            decimal? lowResValue = items?.GetItem(lowResItemNumber)?.NumericValue ?? 0;
            decimal? highResValue = items?.GetItem(highResItemNumber)?.NumericValue ?? 0;

            return JoinLowResHighResReading(lowResValue, highResValue);
        }

        public static decimal GetHighResFractionalValue(decimal highResValue)
        {
            if (highResValue == 0) return 0;

            var highResString = Convert.ToString(highResValue, CultureInfo.InvariantCulture);
            var pointLocation = highResString.IndexOf(".", StringComparison.Ordinal);

            if (highResValue > 0 && pointLocation > -1)
            {
                var result = highResString.Substring(pointLocation, highResString.Length - pointLocation);

                return Convert.ToDecimal(result);
            }

            return 0;
        }

        public static decimal GetHighResolutionItemValue(int lowResValue, decimal highResValue)
        {
            var fractional = GetHighResFractionalValue(highResValue);
            return lowResValue + fractional;
        }

        public static decimal JoinLowResHighResReading(decimal? lowResValue, decimal? highResValue)
        {
            if (!lowResValue.HasValue || !highResValue.HasValue)
                throw new ArgumentNullException(nameof(lowResValue) + " & " + nameof(highResValue));

            return GetHighResolutionItemValue((int) lowResValue.Value, highResValue.Value);
        }
    }
}