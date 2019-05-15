using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Honeywell.Core.Repository.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Devices.Honeywell.Core.Repository
{
    public class JsonDeviceDataSource : IDeviceDataSource<HoneywellDeviceType>
    {
        public JsonDeviceDataSource(IStreamReader streamReader)
        {
            _streamReader = streamReader;
            _itemConverter = new JsonItemsConverter(this);
        }

        public IObservable<HoneywellDeviceType> GetDeviceTypes()
        {
            var converter = new JsonDeviceConverter(GetItems().ToEnumerable(), _itemConverter);

            return _streamReader.GetDeviceTypeReaders().ToObservable()
                .Select(sr => Observable.Start(() =>
                {
                    var serializer = new JsonSerializer();
                    serializer.Converters.Add(converter);
                    using (var reader = new JsonTextReader(sr))
                    {
                        return serializer.Deserialize<HoneywellDeviceType>(reader);
                    }
                }))
                .Merge();
        }

        public IEnumerable<HoneywellDeviceType> GetDeviceTypesEnumerable()
        {
            return GetDeviceTypes().ToEnumerable();
        }

        public async Task<IEnumerable<ItemMetadata.ItemDescription>> GetItemDescriptionsAsync(string name)
        {
            using (var sr = _streamReader.GetItemDefinitionsReader(name))
            {
                using (var reader = new JsonTextReader(sr))
                {
                    var defJObject = await JObject.LoadAsync(reader);

                    var descTypeString = defJObject["type"]?.Value<string>();
                    if (string.IsNullOrEmpty(descTypeString))
                        throw new NullReferenceException($"Could not find [type] property in {name} definition. Verify the JSON document is valid.");

                    var type = Type.GetType(descTypeString);
                    if (type == null)
                        throw new TypeLoadException($"Could not load type for {descTypeString}.");

                    var items = defJObject["ItemDescriptions"]?.ToString();
                    if (items == null)
                        throw new NullReferenceException($"Could not find [ItemDescriptions] property in item type {name}. Verify the JSON document is valid.");

                    return (IEnumerable<ItemMetadata.ItemDescription>)
                        JsonConvert.DeserializeObject(items, typeof(ICollection<>).MakeGenericType(type));
                }
            }
        }

        public IObservable<ItemMetadata> GetItems()
        {
            using (var sr = _streamReader.GetItemsReader())
            using (JsonReader reader = new JsonTextReader(sr))
            {
                var serializer = new JsonSerializer();
                serializer.Converters.Add(_itemConverter);
                return serializer.Deserialize<ICollection<ItemMetadata>>(reader)
                    .ToObservable();
            }
        }

        public IEnumerable<ItemMetadata> GetItemsEnumerable()
        {
            return GetItems().ToEnumerable();
        }

        private JsonItemsConverter _itemConverter;

        private IStreamReader _streamReader;
    }
}