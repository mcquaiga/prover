using System;
using System.Collections.Generic;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prover.Shared.Json;

namespace Devices.Core.Repository.JsonConverters
{
    public abstract class JsonDeviceConverter<T> : JsonCreationConverter<T>
        where T : DeviceType
    {
        #region Fields

        protected readonly IEnumerable<ItemMetadata> GlobalItems;

        private readonly JsonItemsConverter _itemConverter;

        #endregion

        #region Constructors

        public JsonDeviceConverter(IEnumerable<ItemMetadata> globalItems, JsonItemsConverter itemConverter)
        {
            GlobalItems = globalItems;
            _itemConverter = itemConverter;
        }

        #endregion

        #region Methods

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        protected abstract override T Create(Type objectType, JObject jObject);

        protected ICollection<ItemMetadata> DeserializeItems(string attributeName, JObject jObject)
        {
            var json = jObject[attributeName]?.ToString();
            if (string.IsNullOrEmpty(json))
                return default;

            return JsonConvert.DeserializeObject<ICollection<ItemMetadata>>(json, _itemConverter);
        }

        #endregion
    }
}