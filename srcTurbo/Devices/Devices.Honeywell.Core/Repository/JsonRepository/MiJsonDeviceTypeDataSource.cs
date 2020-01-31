using System;
using System.Collections.Generic;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using Devices.Core.Repository.JsonConverters;
using Devices.Honeywell.Core.Repository.JsonRepository.JsonConverters;

namespace Devices.Honeywell.Core.Repository.JsonRepository
{
    public class MiJsonDeviceTypeDataSource : JsonDeviceTypeDataSource<IHoneywellDeviceType>
    {
        private static readonly string _directory = $"{AppDomain.CurrentDomain.BaseDirectory}\\MiDeviceTypes";

        public MiJsonDeviceTypeDataSource(IStreamReader streamReader) : base(streamReader)
        {
        }

        public static IDeviceTypeDataSource<IHoneywellDeviceType> Instance { get; } = _lazy.Value;

        protected static readonly Lazy<IDeviceTypeDataSource<IHoneywellDeviceType>> _lazy
            = new Lazy<IDeviceTypeDataSource<IHoneywellDeviceType>>(Factory);

        protected static IDeviceTypeDataSource<IHoneywellDeviceType> Factory()
        {
            return new MiJsonDeviceTypeDataSource(new FileStreamReader(_directory));
        }

        protected override JsonDeviceConverter<IHoneywellDeviceType> DeviceConverter(IEnumerable<ItemMetadata> items, JsonItemsConverter itemsConverter)
        {
            return new MiJsonDeviceConverter(items, itemsConverter);
        }
    }
}