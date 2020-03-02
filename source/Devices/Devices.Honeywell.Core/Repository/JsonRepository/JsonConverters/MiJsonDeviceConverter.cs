using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Core.Items;
using Devices.Core.Repository.JsonConverters;
using Newtonsoft.Json.Linq;

namespace Devices.Honeywell.Core.Repository.JsonRepository.JsonConverters
{
    public class MiJsonDeviceConverter : JsonDeviceConverter<HoneywellDeviceType>
    {

        public MiJsonDeviceConverter(IEnumerable<ItemMetadata> globalItems, JsonItemsConverter itemConverter) : base(globalItems, itemConverter)
        {
        }
  

        protected override HoneywellDeviceType Create(Type objectType, JObject jObject)
        {
            if (jObject["Items"] != null)
            {
                var items = DeserializeItems("Items", jObject);
                return new HoneywellDeviceType(items);
            }

            var includes = DeserializeItems("OverrideItems", jObject);
            var excludes = DeserializeItems("ExcludeItems", jObject);
            
            return new HoneywellDeviceType(GenerateItemsList(GlobalItems.ToList(), includes, excludes));
        }

        private IEnumerable<ItemMetadata> GenerateItemsList(IEnumerable<ItemMetadata> globalItems,
            IEnumerable<ItemMetadata> overrideItems,
            IEnumerable<ItemMetadata> excludeItems)
        {
            globalItems = globalItems ?? new List<ItemMetadata>();
            overrideItems = overrideItems ?? new List<ItemMetadata>();
            excludeItems = excludeItems ?? new List<ItemMetadata>();

            return globalItems.Concat(overrideItems)
                .Where(item => excludeItems.All(x => x.Number != item.Number))
                .GroupBy(item => item.Number)
                .Select(group => group.Aggregate((_, next) => next))
                .OrderBy(i => i.Number);
        }
    }
}
