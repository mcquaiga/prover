using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.Common.Models;

namespace Prover.CommProtocol.MiHoneywell.Items
{
    public static class ItemHelpers
    {
        private static readonly string ItemDefinitionsFolder = $@"{Environment.CurrentDirectory}\ItemDefinitions";

        private const string ItemDefinitionFileName = "Items.json";
        private const string TypeFileName = "Type_";

        private static readonly List<InstrumentType> ItemFileCache = new List<InstrumentType>();
        private static List<ItemMetadata> _itemDefinitions;

        public static async Task<IEnumerable<InstrumentType>> LoadInstruments()
        {          
            if (ItemFileCache.Any())
                return ItemFileCache;

            return await Task.Run(() =>
            {
                var result = new List<InstrumentType>();
                var items = LoadItemDefinitions().ToList();

                var files = Directory.GetFiles(ItemDefinitionsFolder, $"{TypeFileName}*.json");
                foreach (var file in files)
                {
                    var fileText = File.ReadAllText(file);
                    var instrJson = JObject.Parse(fileText);
                    var i = instrJson.ToObject<InstrumentType>();
                    result.Add(i);

                    var overrideItems = GetItemDefinitions(fileText, "OverrideItems").ToList();
                    var mergedList = items.Concat(overrideItems)
                        .GroupBy(item => item.Number)
                        .Select(group => group.Aggregate((merged, next) => next))
                        .ToList();

                    var excludeItems = GetItemDefinitions(fileText, "ExcludeItems").ToList();
                    excludeItems
                        .ForEach(ei => mergedList.RemoveAll(x => x.Number == ei.Number));

                    i.ItemsMetadata = mergedList;

                    ItemFileCache.Add(i);
                }

                MeterIndexInfo.Get(0);

                return result;
            });
        }

        private static IEnumerable<ItemMetadata> LoadItemDefinitions()
        {
            if (_itemDefinitions != null && _itemDefinitions.Any())
                return _itemDefinitions;

            _itemDefinitions = new List<ItemMetadata>();
            var path = $@"{ItemDefinitionsFolder}\{ItemDefinitionFileName}";           
            var itemText = File.ReadAllText(path);
            _itemDefinitions = GetItemDefinitions(itemText, "ItemDefinitions").ToList();

            return _itemDefinitions;
        }  

        private static IEnumerable<ItemMetadata> GetItemDefinitions(string itemDefinitionText, string attributeName)
        {
            var itemsJson = JObject.Parse(itemDefinitionText);
            var items = itemsJson[attributeName].Children().ToList();
            var results = new List<ItemMetadata>();
            foreach(JToken item in items)
            {                
                var i = item.ToObject<ItemMetadata>();

                var values = item["value"]?.Children().ToList();
                i.ItemDescriptions = values?.Select(v => v.ToObject<ItemMetadata.ItemDescription>());

                results.Add(i);
            }
            return results;
        }

        public static IEnumerable<ItemValue> LoadItems(InstrumentType instrumentType, Dictionary<int, string> itemValues)
        {
            if (instrumentType == null)
                throw new ArgumentNullException(nameof(instrumentType));

            var metadata = instrumentType.ItemsMetadata;

            //return metadata.Select(meta => new ItemValue(meta, itemValues.ContainsKey(meta.Number) ? itemValues[meta.Number] : string.Empty));
            return itemValues
                .Where(i => metadata.GetItem(i.Key) != null)
                .Select(iv => new ItemValue(metadata.GetItem(iv.Key), iv.Value));
        }
    }
}