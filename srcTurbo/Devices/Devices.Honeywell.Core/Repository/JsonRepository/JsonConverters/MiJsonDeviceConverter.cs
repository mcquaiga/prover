using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Core.Items;
using Devices.Core.Repository.JsonConverters;
using Newtonsoft.Json.Linq;

namespace Devices.Honeywell.Core.Repository.JsonRepository.JsonConverters
{
    public class MiJsonDeviceConverter : JsonDeviceConverter<IHoneywellDeviceType>
    {

        public MiJsonDeviceConverter(IEnumerable<ItemMetadata> globalItems, JsonItemsConverter itemConverter) : base(globalItems, itemConverter)
        {
        }
  

        protected override IHoneywellDeviceType Create(Type objectType, JObject jObject)
        {
            if (jObject["Items"] != null)
            {
                var items = DeserializeItems("Items", jObject);
                return new HoneywellDeviceType(items);
            }

            var includes = DeserializeItems("OverrideItems", jObject);
            var excludes = DeserializeItems("ExcludeItems", jObject);

            return new HoneywellDeviceType(GlobalItems.ToList(), includes, excludes);
        }
    }
}
