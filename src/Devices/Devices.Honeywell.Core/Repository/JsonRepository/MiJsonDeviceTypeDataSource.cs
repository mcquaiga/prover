using System;
using System.Collections.Generic;
using Devices.Core.Items;
using Devices.Core.Repository;
using Devices.Core.Repository.JsonConverters;
using Devices.Honeywell.Core.Repository.JsonRepository.JsonConverters;

namespace Devices.Honeywell.Core.Repository.JsonRepository
{
    public class MiJsonDeviceTypeDataSource : JsonDeviceTypeDataSource<HoneywellDeviceType>
    {
        private static readonly string _directory = $"{AppDomain.CurrentDomain.BaseDirectory}\\MiDeviceTypes";

        public MiJsonDeviceTypeDataSource(IStreamReader streamReader) : base(streamReader)
        {
        }

        public static IDeviceTypeDataSource<HoneywellDeviceType> Instance => _lazy.Value;

        protected static readonly Lazy<IDeviceTypeDataSource<HoneywellDeviceType>> _lazy
            = new Lazy<IDeviceTypeDataSource<HoneywellDeviceType>>(Factory);

        protected static IDeviceTypeDataSource<HoneywellDeviceType> Factory()
        {
            return new MiJsonDeviceTypeDataSource(new FileStreamReader(_directory));
        }

        protected override JsonDeviceConverter<HoneywellDeviceType> DeviceConverter(IEnumerable<ItemMetadata> items, JsonItemsConverter itemsConverter)
        {
            return new MiJsonDeviceConverter(items, itemsConverter);
        }
    }
}