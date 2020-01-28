using Devices.Core.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Json;

namespace Devices.Honeywell.Core.Repository.JsonConverters
{
    internal class JsonDeviceConverter : JsonCreationConverter<HoneywellDeviceType>
    {
        #region Fields

        private readonly IEnumerable<ItemMetadata> _globalItems;

        private readonly JsonItemsConverter _itemConverter;

        #endregion

        #region Constructors

        public JsonDeviceConverter(IEnumerable<ItemMetadata> globalItems, JsonItemsConverter itemConverter)
        {
            _globalItems = globalItems;
            _itemConverter = itemConverter;
        }

        #endregion

        #region Methods

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
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

            return new HoneywellDeviceType(_globalItems.ToList(), includes, excludes);
        }

        private ICollection<ItemMetadata> DeserializeItems(string attributeName, JObject jObject)
        {
            var json = jObject[attributeName]?.ToString();
            if (string.IsNullOrEmpty(json))
                return default;

            return JsonConvert.DeserializeObject<ICollection<ItemMetadata>>(json, _itemConverter);
        }

        #endregion
    }
}