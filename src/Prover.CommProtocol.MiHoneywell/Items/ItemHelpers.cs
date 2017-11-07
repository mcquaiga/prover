using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NuGet;
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

        private static HashSet<InstrumentType> _instrumentTypesCache = new HashSet<InstrumentType>();

        public static async Task<HashSet<InstrumentType>> GetInstrumentDefinitions()
        {
            if (!_instrumentTypesCache.Any())
                await LoadInstrumentTypes();

            return _instrumentTypesCache;
        }

        public static async Task LoadInstrumentTypes()
        {
            _instrumentTypesCache.Clear();

            var items = await LoadGlobalItemDefinitions();

            var files = Directory.GetFiles(ItemDefinitionsFolder, $"{TypeFileName}*.json");
            foreach (var file in files)
            {
                var fileText = File.ReadAllText(file);
                var instrJson = JObject.Parse(fileText);
                var i = instrJson.ToObject<InstrumentType>();

                var overrideItems = await GetItemDefinitions(fileText, "OverrideItems");
                var excludeItems = await GetItemDefinitions(fileText, "ExcludeItems");
                i.ItemsMetadata = items.Concat(overrideItems)
                    .Where(item => excludeItems.All(x => x.Number != item.Number))
                    .GroupBy(item => item.Number)                        
                    .Select(group => group.Aggregate((merged, next) => next))
                    .ToList();

                _instrumentTypesCache.Add(i);
            }
        }

        private static async Task<HashSet<ItemMetadata>> LoadGlobalItemDefinitions()
        {
            var path = $@"{ItemDefinitionsFolder}\{ItemDefinitionFileName}";           
            var itemText = File.ReadAllText(path);
            return await GetItemDefinitions(itemText, "ItemDefinitions");            
        }  

        private static async Task<HashSet<ItemMetadata>> GetItemDefinitions(string itemDefinitionText, string attributeName)
        {
            var itemsJson = JObject.Parse(itemDefinitionText);
            var items = itemsJson[attributeName].Children().ToList();
            var results = new HashSet<ItemMetadata>();
            foreach(JToken item in items)
            {                
                var i = item.ToObject<ItemMetadata>();

                JToken itemValues;
                if (item["definitionPath"] != null)
                {
                    var defPath = item["definitionPath"].Value<string>();
                    var definitionsJObject = await ReadItemFile(defPath);

                    var typeName = definitionsJObject["type"].Value<string>();
                    var type = Type.GetType(typeName);
                    itemValues = definitionsJObject["values"];
                    var values = itemValues?.Children().ToList();
                    i.ItemDescriptions = values?.Select(v => (ItemMetadata.ItemDescription)v.ToObject(type));
                }
                else
                {
                    itemValues = item["values"];
                    var values = itemValues?.Children().ToList();
                    i.ItemDescriptions = values?.Select(v => v.ToObject<ItemMetadata.ItemDescription>());
                }

                results.Add(i);
            }
            return results;
        }

        private static async Task<JObject> ReadItemFile(string fileName)
        {
            var path = $@"{ItemDefinitionsFolder}\{fileName}";
            using (var fs = File.OpenText(path))
            {
                var t = await fs.ReadToEndAsync();
                return JObject.Parse(t);
            }
        }

        public static IEnumerable<ItemValue> LoadItems(InstrumentType instrumentType, Dictionary<int, string> itemValues)
        {
            if (instrumentType == null)
                throw new ArgumentNullException(nameof(instrumentType));

            var metadata = instrumentType.ItemsMetadata;

            return itemValues
                .Where(i => metadata.GetItem(i.Key) != null)
                .Select(iv => new ItemValue(metadata.GetItem(iv.Key), iv.Value));
        }
    }
}