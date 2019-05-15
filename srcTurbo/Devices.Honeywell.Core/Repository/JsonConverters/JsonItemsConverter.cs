using Core.Extensions;
using Core.Json;
using Devices.Core.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Devices.Honeywell.Core.Repository.JsonConverters
{
    internal class JsonItemsConverter : JsonCreationConverter<ItemMetadata>
    {
        public JsonItemsConverter(JsonDeviceDataSource itemDataSource)
        {
            _itemDataSource = itemDataSource;
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
                    var task = Task.Run(async () => await _itemDataSource.GetItemDescriptionsAsync(itemName));
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

        private readonly JsonDeviceDataSource _itemDataSource;
    }
}