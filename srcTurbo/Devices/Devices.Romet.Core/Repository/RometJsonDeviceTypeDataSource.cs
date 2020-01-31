using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using Devices.Core.Repository.JsonConverters;
using Devices.Honeywell.Core;
using Devices.Honeywell.Core.Repository;
using Newtonsoft.Json.Linq;

namespace Devices.Romet.Core.Repository
{
    public class RometJsonDeviceTypeDataSource : JsonDeviceTypeDataSource<IRometDeviceType>
    {
        private static readonly string _directory = $"{AppDomain.CurrentDomain.BaseDirectory}\\RometDeviceTypes";

        private RometJsonDeviceTypeDataSource() 
            : base(new FileStreamReader(_directory))
        {
        }

        public RometJsonDeviceTypeDataSource(IStreamReader streamReader) : base(streamReader)
        {
        }

        private static readonly Lazy<IDeviceTypeDataSource<IRometDeviceType>> _lazy
            = new Lazy<IDeviceTypeDataSource<IRometDeviceType>>(Factory);

        public static IDeviceTypeDataSource<IRometDeviceType> Instance { get; } = _lazy.Value;

        private static IDeviceTypeDataSource<IRometDeviceType> Factory()
        {
            return new RometJsonDeviceTypeDataSource();
        }

        protected override JsonDeviceConverter<IRometDeviceType> DeviceConverter(IEnumerable<ItemMetadata> items, JsonItemsConverter itemsConverter)
        {
            return new RometJsonDeviceConverter(items, itemsConverter);
        }
    }

    public class RometJsonDeviceConverter : JsonDeviceConverter<IRometDeviceType>
    {
        public RometJsonDeviceConverter(IEnumerable<ItemMetadata> globalItems, JsonItemsConverter itemConverter) : base(globalItems, itemConverter)
        {
        }

        protected override IRometDeviceType Create(Type objectType, JObject jObject)
        {
            if (jObject["Items"] != null)
            {
                var items = DeserializeItems("Items", jObject);
                return new RometDeviceType(items);
            }

            var includes = DeserializeItems("OverrideItems", jObject);
            var excludes = DeserializeItems("ExcludeItems", jObject);

            return new RometDeviceType(GlobalItems.ToList(), includes, excludes);
        }
    }
}
