using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Devices.Core.Repository
{
    public abstract class JsonDeviceTypeDataSource<T> : IDeviceTypeDataSource<T>
        where T : IDeviceType
    {

        protected JsonDeviceTypeDataSource(IStreamReader streamReader)
        {
            _streamReader = streamReader;
            _itemConverter = new JsonItemsConverter(_streamReader);
        }

        private readonly JsonItemsConverter _itemConverter;
        private readonly IStreamReader _streamReader;

        public IObservable<T> GetDeviceTypes()
        {
            var globalItems = GetItems().ToEnumerable();

            var converter = DeviceConverter(globalItems, _itemConverter);

            return _streamReader.GetDeviceTypeReaders().ToObservable()
                .Select(sr => Observable.Start(() =>
                {
                    var serializer = new JsonSerializer();
                    serializer.Converters.Add(converter);
                    using (var reader = new JsonTextReader(sr))
                    {
                        return serializer.Deserialize<T>(reader);
                    }
                }))
                .Merge();
        }

        protected abstract JsonDeviceConverter<T> DeviceConverter(IEnumerable<ItemMetadata> items,
            JsonItemsConverter itemsConverter);

        public IObservable<ItemMetadata> GetItems()
        {
            using (var sr = _streamReader.GetItemsReader())
            {
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    var serializer = new JsonSerializer();
                    serializer.Converters.Add(_itemConverter);
                    return serializer.Deserialize<ICollection<ItemMetadata>>(reader)
                        .ToObservable();
                }
            }
        }

        public IEnumerable<T> GetDeviceTypesEnumerable()
        {
            return GetDeviceTypes().ToEnumerable();
        }

        public IEnumerable<ItemMetadata> GetItemsEnumerable()
        {
            return GetItems().ToEnumerable();
        }
    }
}