using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Prover.CommProtocol.Common.Instruments;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell.CommClients;
using Prover.Shared.Enums;

namespace Prover.CommProtocol.MiHoneywell.Instruments
{
    public enum HoneywellInstrumentType
    {
        MiniAt,
        MiniMax
    }

    public static class HoneywellInstruments
    {
        public static async Task<IInstrument> CreateInstrument(CommPort commPort, HoneywellInstrumentType instrumentType)
        {
            var commClient = new HoneywellClient(commPort);

            HoneywellInstrument instrument;
            if (instrumentType == HoneywellInstrumentType.MiniMax)
                instrument = new MiniMaxInstrument(commClient);
            else
                instrument = new MiniAtInstrument(commClient);

            await instrument.DownloadItemFile();
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
            CommClient.Instrument = this;
            ItemDefinitions = Items.LoadItems(this);
        }

        public int AccessCode { get; protected set; }
        public string ItemFilePath { get; protected set; }
        public IEnumerable<ItemValue> ItemValues { get; protected set; }

        public int Id { get; protected set; }
        public string Name { get; protected set; }
        public IEnumerable<ItemMetadata> ItemDefinitions { get; }

        public EvcCorrectorType CorrectorType()
        {
            var live = "Live";
            var pressureLive = GetItemValue(ItemCodes.Pressure.FixedFactor, ItemValues).Description == live;
            var tempLive = GetItemValue(ItemCodes.Temperature.FixedFactor, ItemValues).Description == live;
            var superLive = GetItemValue(ItemCodes.Super.FixedFactor, ItemValues).Description == live;

            if (pressureLive && tempLive && superLive)
                return EvcCorrectorType.PTZ;

            if (pressureLive)
                return EvcCorrectorType.P;

            if (tempLive)
                return EvcCorrectorType.T;

            throw new NotSupportedException("Could not determine the corrector type.");
        }

        public Dictionary<string, string> ItemData
            => ItemValues.ToDictionary(k => k.Metadata.Number.ToString(), v => v.RawValue);

        public ISiteInformationItems SiteInformationItems => new SiteInformation(this);

        public virtual ITemperatureItems TemperatureItems => new Temperature(this);

        public virtual IPressureItems PressureItems => new Pressure(this);

        public virtual ISuperFactorItems SuperFactorItems => new SuperFactor(this);

        public abstract IVolumeItems VolumeItems { get; }

        public async Task<ITemperatureItems> DownloadTemperatureItems()
        {
            var items = await CommClient.GetItemValues(ItemDefinitions.TemperatureItems());
            return new Temperature(items);
        }

        public async Task<IPressureItems> DownloadPressureItems()
        {
            var items = await CommClient.GetItemValues(ItemDefinitions.PressureItems());
            return new Pressure(items);
        }

        public abstract Task<IVolumeItems> DownloadVolumeItems();

        public DriveTypeDescripter DriveType
            => GetItemValue(98).Description == "Rotary" ? DriveTypeDescripter.Rotary : DriveTypeDescripter.Mechanical;

        public async Task DownloadItemFile()
        {
            await CommClient.Connect(CancellationToken.None);
            ItemValues = await CommClient.GetItemValues(ItemDefinitions.GetAllItemNumbers());
            await CommClient.Disconnect();
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

        private ItemValue GetItemValue(int itemNumber)
        {
            var item = ItemValues.ToList().FirstOrDefault(i => i.Metadata.Number == itemNumber);
            return item;
        }

        public abstract IRotaryMeterItems RotaryItems { get; }
        public abstract IEnergyItems EnergyItems { get; }

        internal class SiteInformation : ISiteInformationItems
        {
            private readonly HoneywellInstrument _instrument;

            public SiteInformation(HoneywellInstrument instrument)
            {
                _instrument = instrument;
            }

            public string SerialNumber => _instrument.GetItemValue(62).Description;
            public string CompanyNumber => _instrument.GetItemValue(201).Description;
            public string FirmwareVersion => _instrument.GetItemValue(122).Description;
        }

        internal class Temperature : ITemperatureItems
        {
            private readonly IEnumerable<ItemValue> _itemValues;

            public Temperature(IEnumerable<ItemValue> itemValues)
            {
                _itemValues = itemValues;
            }

            public Temperature(HoneywellInstrument instrument) : this(instrument.ItemValues)
            {
            }

            public decimal Base => _itemValues.GetItem(34).NumericValue;

            public TemperatureUnits Units
                => (TemperatureUnits) Enum.Parse(typeof(TemperatureUnits), _itemValues.GetItem(89).Description);

            public decimal GasTemperature => _itemValues.GetItem(26).NumericValue;
            public decimal Factor => _itemValues.GetItem(45).NumericValue;

            public void Update(ITemperatureItems temperatureItems)
            {
                throw new NotImplementedException();
            }
        }

        internal class Pressure : IPressureItems
        {
            private readonly IEnumerable<ItemValue> _itemValues;

            public Pressure(IEnumerable<ItemValue> itemValues)
            {
                _itemValues = itemValues;
            }

            public Pressure(HoneywellInstrument instrument) : this(instrument.ItemValues)
            {
            }

            public int Range => (int) _itemValues.GetItem(137).NumericValue;
            public string TransducerType => _itemValues.GetItem(112).Description;
            public decimal Base => _itemValues.GetItem(13).NumericValue;
            public decimal GasPressure => _itemValues.GetItem(8).NumericValue;
            public decimal AtmPressure => _itemValues.GetItem(14).NumericValue;
            public decimal Factor => _itemValues.GetItem(44).NumericValue;

            public void Update(IPressureItems pressureItems)
            {
                throw new NotImplementedException();
            }
        }

        internal class SuperFactor : ISuperFactorItems
        {
            private readonly IEnumerable<ItemValue> _itemValues;

            public SuperFactor(IEnumerable<ItemValue> itemValues)
            {
                _itemValues = itemValues;
            }

            public SuperFactor(HoneywellInstrument instrument) : this(instrument.ItemValues)
            {
            }

            public decimal SpecGr => _itemValues.GetItem(53).NumericValue;
            public decimal N2 => _itemValues.GetItem(54).NumericValue;
            public decimal Co2 => _itemValues.GetItem(55).NumericValue;
        }
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