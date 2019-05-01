using Core.Extensions;
using Core.Json;
using Devices.Core.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Devices.Honeywell.Core.Repository.JsonConverters
{
    internal class JsonItemsConverter : JsonCreationConverter<ItemMetadata>
    {
        #region Constructors

        public JsonItemsConverter(JsonDeviceDataSource itemDataSource)
        {
            _itemDataSource = itemDataSource;
        }

        #endregion

        #region Fields

        private readonly JsonDeviceDataSource _itemDataSource;

        #endregion

        #region Methods

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
                    return new ItemMetadata(
                        AsyncUtil.RunSync(() => _itemDataSource.GetItemDescriptions(itemName)).ToList()
                    );
                }
                else
                {
                    return new ItemMetadata(itemToken.ToObject<ICollection<ItemMetadata.ItemDescription>>());
                }
            }

            return new ItemMetadata();
        }

        #endregion
    }
}