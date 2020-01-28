using Devices.Core.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Shared.Json;

namespace Devices.Honeywell.Core.Repository.JsonConverters
{
    internal class JsonItemsConverter : JsonCreationConverter<ItemMetadata>
    {
        public JsonItemsConverter(JsonDeviceTypeDataSource itemTypeDataSource)
        {
            _itemTypeDataSource = itemTypeDataSource;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        protected override ItemMetadata Create(Type objectType, JObject jObject)
        {
            var itemToken = jObject["ItemDescriptions"];
            if (itemToken != null && itemToken.HasValues)
            {
                var itemName = itemToken.First.Value<string>("definitionPath");
                if (itemName != null)
                {
                    var task = Task.Run(async () => await _itemTypeDataSource.GetItemDescriptionsAsync(itemName));
                    var desc = task.Result;
                    return new ItemMetadata(desc.ToList());
                }
                else
                {
                    return new ItemMetadata(itemToken.ToObject<ICollection<ItemMetadata.ItemDescription>>());
                }
            }

            return new ItemMetadata();
        }

        private readonly JsonDeviceTypeDataSource _itemTypeDataSource;
    }
}