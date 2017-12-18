using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell.CommClients;

namespace Prover.CommProtocol.MiHoneywell.Items
{
    public static class ItemHelpers
    {
        private static readonly string ItemDefinitionsFolder = $@"{Environment.CurrentDirectory}\ItemDefinitions";

        private const string ItemDefinitionFileName = "Items.json";
        private const string TypeFileName = "Type_";

        private static HashSet<InstrumentType> _instrumentTypesCache;

        public static async Task<HashSet<InstrumentType>> GetInstrumentDefinitions()
        {
            if (_instrumentTypesCache == null || !_instrumentTypesCache.Any())
                await LoadInstrumentTypes();

            return _instrumentTypesCache;
        }

        public static async Task LoadInstrumentTypes()
        {
            if (_instrumentTypesCache == null)
                _instrumentTypesCache = new HashSet<InstrumentType>();

            _instrumentTypesCache.Clear();

            var items = await LoadGlobalItemDefinitions();

            var files = Directory.GetFiles(ItemDefinitionsFolder, $"{TypeFileName}*.json");
            foreach (var file in files)
            {
                var fileText = File.ReadAllText(file);
                var instrJson = JObject.Parse(fileText);
                var i = instrJson.ToObject<InstrumentType>();
                i.ClientFactory = GetCommClientFactory(i);
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

        private static Func<ICommPort, ISubject<string>, EvcCommunicationClient> GetCommClientFactory(InstrumentType instrumentType)
        {
            var commTypeName = instrumentType.CommClientType;
            if (string.IsNullOrEmpty(commTypeName))
                commTypeName = "Prover.CommProtocol.MiHoneywell.CommClients.HoneywellClient";

            var commType = Type.GetType(commTypeName);

            if (commType == null)
                commType = typeof(HoneywellClient);

            return (port, statusSubject) => (EvcCommunicationClient) Activator.CreateInstance(commType, port, instrumentType, statusSubject);
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
            var items = itemsJson[attributeName]?.Children().ToList();

            var results = new HashSet<ItemMetadata>();
            if (items == null) return results;

            foreach (JToken item in items)
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
                    i.ItemDescriptions = values?.Select(v => (ItemMetadata.ItemDescription) v.ToObject(type));
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

            return itemValues
                .Where(i => instrumentType.ItemsMetadata.GetItem(i.Key) != null)
                .Select(iv => new ItemValue(instrumentType.ItemsMetadata.GetItem(iv.Key), iv.Value));
        }
    }
}