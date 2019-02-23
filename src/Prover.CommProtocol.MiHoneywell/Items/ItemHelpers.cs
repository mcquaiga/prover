using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NLog;
using NuGet;
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
            var readTasks = new List<Task>();
            var results = new ConcurrentDictionary<InstrumentType, byte>();
            var files = Directory.GetFiles(ItemDefinitionsFolder, $"{TypeFileName}*.json");
            foreach (var file in files)
            {
                readTasks.Add(
                    Task.Run(async () =>
                    {
                        var instrJson = await FileTextToJObjectAsync(file);
                        var i = instrJson.ToObject<InstrumentType>();
                        i.ClientFactory = GetCommClientFactory(i);

                        var overrideItems = await GetItemDefinitions(instrJson, "OverrideItems");
                        var excludeItems = await GetItemDefinitions(instrJson, "ExcludeItems");

                        i.ItemsMetadata = items.Concat(overrideItems)
                            .Where(item => excludeItems.All(x => x.Number != item.Number))
                            .GroupBy(item => item.Number)
                            .Select(group => group.Aggregate((merged, next) => next))
                            .ToList();
                        results.TryAdd(i, new byte());
                        //lock(_instrumentTypesCache) _instrumentTypesCache.Add(i);
                    })
                );
            }

            await Task.WhenAny(readTasks.ToArray())
                .ContinueWith(task => LogManager.GetCurrentClassLogger().Debug("Loading Honeywell instrument types"));
            await Task.WhenAll(readTasks.ToArray())
                .ContinueWith(task =>
                {
                    _instrumentTypesCache.AddRange(results.Select(i => i.Key).ToList());
                    LogManager.GetCurrentClassLogger().Debug("Finished loading Honeywell instrument definitions.");
                });
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
            return await GetItemDefinitions(path, "ItemDefinitions");            
        }

        private static async Task<HashSet<ItemMetadata>> GetItemDefinitions(JObject itemsJObject, string attributeName)
        {
            var results = new HashSet<ItemMetadata>();
            var items = itemsJObject[attributeName]?.Children().ToList();            
            if (items == null) return results;

            var tasks = new List<Task>();
            foreach (var item in items)
            {
                var task = Task.Run(async () =>
                {
                    var i = item.ToObject<ItemMetadata>();

                    JToken itemValues;
                    if (item["definitionPath"] != null)
                    {
                        var defPath = $@"{ItemDefinitionsFolder}\{item["definitionPath"].Value<string>()}";
                        var definitionsJObject = await FileTextToJObjectAsync(defPath);

                        var typeName = definitionsJObject["type"].Value<string>();
                        var type = Type.GetType(typeName);
                        if (type == null)
                            throw new NullReferenceException();

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

                    lock (results)
                    {
                        results.Add(i);
                    }
                });

                tasks.Add(task);
            }

            await Task.WhenAll(tasks.ToArray());

            return results;
        }

        private static async Task<HashSet<ItemMetadata>> GetItemDefinitions(string itemFilePath, string attributeName)
        {
            var itemsJson = await FileTextToJObjectAsync(itemFilePath);
            return await GetItemDefinitions(itemsJson, attributeName);
        }

        private static async Task<JObject> FileTextToJObjectAsync(string path)
        {            
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