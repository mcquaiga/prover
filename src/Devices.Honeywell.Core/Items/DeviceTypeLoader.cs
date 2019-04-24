namespace Prover.CommProtocol.MiHoneywell.Items
{
    using Devices.Core.Interfaces;
    using Devices.Core.Items;
    using Devices.Honeywell.Core;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="DeviceTypeLoader"/>
    /// </summary>
    public class DeviceTypeLoader : ILoadDeviceTypes<IHoneywellEvcType>
    {
        #region Constants

        /// <summary>
        /// Defines the ItemDefinitionFileName
        /// </summary>
        private const string ItemDefinitionFileName = "Items.json";

        /// <summary>
        /// Defines the TypeFileName
        /// </summary>
        private const string TypeFileName = "Type_";

        #endregion Constants

        #region Fields

        public IReadOnlyCollection<IHoneywellEvcType> EvcDeviceTypes => _evcTypesCache;

        /// <summary>
        /// Defines the ItemDefinitionsFolder
        /// </summary>
        private static readonly string ItemDefinitionsFolder = $@"{Environment.CurrentDirectory}\ItemDefinitions";

        private List<IHoneywellEvcType> _evcTypesCache = new List<IHoneywellEvcType>();

        #endregion Fields

        #region Methods

        public async Task<IEnumerable<IHoneywellEvcType>> LoadDevicesAsync()
        {
            if (_evcTypesCache.Count == 0)
            {
                await LoadInstrumentTypes();
            }

            return _evcTypesCache;
        }

        /// <summary>
        /// The FileTextToJObjectAsync
        /// </summary>
        /// <param name="path">The path <see cref="string"/></param>
        /// <returns>The <see cref="Task{JObject}"/></returns>
        private static async Task<JObject> FileTextToJObjectAsync(string path)
        {
            using (var fs = File.OpenText(path))
            {
                var t = await fs.ReadToEndAsync();
                return JObject.Parse(t);
            }
        }

        private static async Task GetInstrument(HashSet<ItemMetadata> items, ConcurrentBag<IHoneywellEvcType> results, string file)
        {
            var instrJson = await FileTextToJObjectAsync(file);
            var i = instrJson.ToObject<HoneywellEvcType>();

            if (instrJson["ItemDefinitions"] != null)
            {
                var newItems = await GetItemDefinitions(instrJson, "ItemDefinitions");
                i.Definitions = newItems.ToList();
            }
            else
            {
                var overrideItems = await GetItemDefinitions(instrJson, "OverrideItems");
                var excludeItems = await GetItemDefinitions(instrJson, "ExcludeItems");

                i.Definitions = items.Concat(overrideItems)
                    .Where(item => excludeItems.All(x => x.Number != item.Number))
                    .GroupBy(item => item.Number)
                    .Select(group => group.Aggregate((merged, next) => next))
                    .ToList();
            }

            results.Add(i);
        }

        /// <summary>
        /// The GetItemDefinitions
        /// </summary>
        /// <param name="itemsJObject">The itemsJObject <see cref="JObject"/></param>
        /// <param name="attributeName">The attributeName <see cref="string"/></param>
        /// <returns>The <see cref="Task{HashSet{ItemMetadata}}"/></returns>
        private static async Task<HashSet<ItemMetadata>> GetItemDefinitions(JObject itemsJObject, string attributeName)
        {
            try
            {
                var items = itemsJObject[attributeName]?.Children().ToList();
                if (items == null)
                {
                    return new HashSet<ItemMetadata>();
                }

                var tasks = new List<Task<ItemMetadata>>();
                foreach (var item in items)
                {
                    tasks.Add(GetItems(item));
                }

                await Task.WhenAll(tasks.ToArray());

                return new HashSet<ItemMetadata>(tasks.Select(i => i.Result));
            }
            catch (Exception)
            {
                //LogManager.GetCurrentClassLogger().Error(ex);
                throw;
            }

            async Task<ItemMetadata> GetItems(JToken item)
            {
                try
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
                        {
                            throw new NullReferenceException($"Type {typeName} does not exist in the project. File {defPath}");
                        }

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

                    return i;
                }
                catch (Exception)
                {
                    //LogManager.GetCurrentClassLogger().Error(ex);
                    throw;
                }
            }
        }

        //    return (port, statusSubject) => (EvcCommunicationClient)Activator.CreateInstance(commType, port, instrumentType, statusSubject);
        //}
        /// <summary>
        /// The GetItemDefinitions
        /// </summary>
        /// <param name="itemFilePath">The itemFilePath <see cref="string"/></param>
        /// <param name="attributeName">The attributeName <see cref="string"/></param>
        /// <returns>The <see cref="Task{HashSet{ItemMetadata}}"/></returns>
        private static async Task<HashSet<ItemMetadata>> GetItemDefinitions(string itemFilePath, string attributeName)
        {
            var itemsJson = await FileTextToJObjectAsync(itemFilePath);
            return await GetItemDefinitions(itemsJson, attributeName);
        }

        // if (commType == null) commType = typeof(HoneywellClient);
        /// <summary>
        /// The LoadGlobalItemDefinitions
        /// </summary>
        /// <returns>The <see cref="Task{HashSet{ItemMetadata}}"/></returns>
        private static Task<HashSet<ItemMetadata>> LoadGlobalItemDefinitions()
        {
            var path = $@"{ItemDefinitionsFolder}\{ItemDefinitionFileName}";
            return GetItemDefinitions(path, "ItemDefinitions");
        }

        /// <summary>
        /// The LoadItems
        /// </summary>
        /// <param name="instrumentType">The instrumentType <see cref="InstrumentType"/></param>
        /// <param name="itemValues">The itemValues <see cref="Dictionary{int, string}"/></param>
        /// <returns>The <see cref="IEnumerable{ItemValue}"/></returns>
        private static IEnumerable<ItemValue> LoadItems(IHoneywellEvcType instrumentType, Dictionary<int, string> itemValues)
        {
            if (instrumentType == null)
            {
                throw new ArgumentNullException(nameof(instrumentType));
            }

            return itemValues
                .Where(i => instrumentType.Definitions.GetItem(i.Key) != null)
                .Select(iv => new ItemValue(instrumentType.Definitions.GetItem(iv.Key), iv.Value));
        }

        /// <summary>
        /// The LoadInstrumentTypes
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        private async Task LoadInstrumentTypes()
        {
            try
            {
                var sw = Stopwatch.StartNew();

                _evcTypesCache.Clear();

                var items = await LoadGlobalItemDefinitions();

                var resultBag = new ConcurrentBag<IHoneywellEvcType>();
                var readTasks = new List<Task>();

                foreach (var file in Directory.GetFiles(ItemDefinitionsFolder, $"{TypeFileName}*.json"))
                {
                    readTasks.Add(GetInstrument(items, resultBag, file));
                }

                await Task.WhenAll(readTasks.ToArray())
                    .ContinueWith(_ =>
                    {
                        sw.Stop();
                        //LogManager.GetCurrentClassLogger().Debug($"Finished loading Honeywell instruments in {sw.ElapsedMilliseconds} ms");
                    });

                _evcTypesCache = resultBag.ToList();
            }
            catch (Exception)
            {
                //LogManager.GetCurrentClassLogger().Error(ex);
                throw;
            }
        }

        ///// <summary>
        ///// The GetCommClientFactory
        ///// </summary>
        ///// <param name="instrumentType">The instrumentType<see cref="InstrumentType"/></param>
        ///// <returns>The <see cref="Func{ICommPort, ISubject{string}, EvcCommunicationClient}"/></returns>
        //private static Func<ICommPort, ISubject<string>, EvcCommunicationClient> GetCommClientFactory(IEvcDeviceInfo instrumentType)
        //{
        //    var commTypeName = instrumentType.CommClientType;
        //    if (string.IsNullOrEmpty(commTypeName))
        //        commTypeName = typeof(HoneywellClient).AssemblyQualifiedName;

        // var commType = Type.GetType(commTypeName);

        #endregion Methods
    }
}