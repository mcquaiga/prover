using System;
using System.Collections.Generic;
using System.Text;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using Devices.Core.Repository.JsonConverters;
using Devices.Honeywell.Core.Repository;

namespace Devices.Romet.Core.Repository
{
    public static class RometDeviceRepository
    {
        public static IDeviceTypeDataSource<DeviceType> DataSource => RometJsonDeviceTypeDataSource.Instance;

        public static DeviceRepository Devices => _lazy.Value;
        private static readonly Lazy<DeviceRepository> _lazy = new Lazy<DeviceRepository>(Factory);
        
        private static DeviceRepository Factory()
        {
            return DeviceRepository.Instance.RegisterDataSource(DataSource);
        }
    }
    public class RometJsonDeviceTypeDataSource : JsonDeviceTypeDataSource<RometDeviceType>
    {
        private static readonly string _directory = $"{AppDomain.CurrentDomain.BaseDirectory}\\RometDeviceTypes";

        private RometJsonDeviceTypeDataSource() 
            : base(new FileStreamReader(_directory))
        {
        }

        public RometJsonDeviceTypeDataSource(IStreamReader streamReader) : base(streamReader)
        {
        }

        private static readonly Lazy<IDeviceTypeDataSource<RometDeviceType>> _lazy
            = new Lazy<IDeviceTypeDataSource<RometDeviceType>>(Factory);

        public static IDeviceTypeDataSource<RometDeviceType> Instance { get; } = _lazy.Value;

        private static IDeviceTypeDataSource<RometDeviceType> Factory()
        {
            return new RometJsonDeviceTypeDataSource();
        }

        protected override JsonDeviceConverter<RometDeviceType> DeviceConverter(IEnumerable<ItemMetadata> items, JsonItemsConverter itemsConverter)
        {
            return new RometJsonDeviceConverter(items, itemsConverter);
        }
    }
}
