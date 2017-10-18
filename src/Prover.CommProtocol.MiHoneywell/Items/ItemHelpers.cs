using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;

namespace Prover.CommProtocol.MiHoneywell.Items
{
    public static class ItemHelpers
    {
        private static readonly string ItemDefinitionsFolder = $@"{Environment.CurrentDirectory}\ItemDefinitions";

        private const string ItemDefinitionFileName = "Items.xml";
        private const string TypeFileName = "Type_";

        private static readonly ConcurrentDictionary<InstrumentType, IEnumerable<ItemMetadata>> ItemFileCache = new ConcurrentDictionary<InstrumentType, IEnumerable<ItemMetadata>>();
        private static List<ItemMetadata> _itemDefinitions;

        public static IEnumerable<InstrumentType> LoadInstruments()
        {
            if (!ItemFileCache.IsEmpty)
                return ItemFileCache.Keys;

            var result = new List<InstrumentType>();
            var items = LoadItemDefinitions().ToList();

            var files = Directory.GetFiles(ItemDefinitionsFolder, $"{TypeFileName}*");
            foreach (var file in files)
            {
                var xDoc = XDocument.Load(file);
                var root = xDoc.XPathSelectElement("InstrumentDetails");
                var i = new InstrumentType()
                {
                    Id = int.Parse(root.XPathSelectElement("Id").Value),
                    Name = root.XPathSelectElement("Name").Value,
                    AccessCode = int.Parse(root.XPathSelectElement("AccessCode").Value),
                    ItemFilePath = (new FileInfo(file).Name),
                    MaxBaudRate = root.XPathSelectElement("MaxBaudRate") != null ? Convert.ToInt32(root.XPathSelectElement("MaxBaudRate").Value) : default(int?),
                    CanUseIrDa = root.XPathSelectElement("CanUserIrDAPort") != null ? Convert.ToBoolean(root.XPathSelectElement("CanUserIrDAPort").Value) : default(bool?)
                };
                result.Add(i);
                ItemFileCache.GetOrAdd(i, items);
            }

            return result;
        }

        private static IEnumerable<ItemMetadata> LoadItemDefinitions()
        {
            if (_itemDefinitions != null && _itemDefinitions.Any())
                return _itemDefinitions;

            _itemDefinitions = new List<ItemMetadata>();
            var path = $@"{ItemDefinitionsFolder}\{ItemDefinitionFileName}";
            var xDoc = XDocument.Load(path);
            _itemDefinitions = (from x in xDoc.Descendants("item")
                                select new ItemMetadata
                                {
                                    Number = Convert.ToInt32(x.Attribute("number").Value),
                                    Code = x.Attribute("code") == null ? "" : x.Attribute("code").Value,
                                    ShortDescription =
                                        x.Attribute("shortDescription") == null ? "" : x.Attribute("shortDescription").Value,
                                    LongDescription = x.Attribute("description") == null ? "" : x.Attribute("description").Value,
                                    IsAlarm = (x.Attribute("isAlarm") != null) && Convert.ToBoolean(x.Attribute("isAlarm").Value),
                                    IsPressure =
                                        (x.Attribute("isPressure") != null) && Convert.ToBoolean(x.Attribute("isPressure").Value),
                                    IsPressureTest =
                                        (x.Attribute("isPressureTest") != null) &&
                                        Convert.ToBoolean(x.Attribute("isPressureTest").Value),
                                    IsTemperature =
                                        (x.Attribute("isTemperature") != null) && Convert.ToBoolean(x.Attribute("isTemperature").Value),
                                    IsTemperatureTest =
                                        (x.Attribute("isTemperatureTest") != null) &&
                                        Convert.ToBoolean(x.Attribute("isTemperatureTest").Value),
                                    IsVolume = (x.Attribute("isVolume") != null) && Convert.ToBoolean(x.Attribute("isVolume").Value),
                                    IsVolumeTest =
                                        (x.Attribute("isVolumeTest") != null) && Convert.ToBoolean(x.Attribute("isVolumeTest").Value),
                                    IsSuperFactor = (x.Attribute("isSuper") != null) && Convert.ToBoolean(x.Attribute("isSuper").Value),
                                    ItemDescriptions =
                                        (from y in x.Descendants("value")
                                         select new ItemMetadata.ItemDescription
                                         {
                                             Id = Convert.ToInt32(y.Attribute("id").Value),
                                             Description = y.Attribute("description").Value,
                                             Value =
                                        y.Attribute("numericvalue") == null
                                            ? (decimal?)null
                                            : Convert.ToDecimal(y.Attribute("numericvalue").Value)
                                         })
                                            .ToList()
                                }
            ).ToList();

            return _itemDefinitions;
        }

        public static IEnumerable<ItemValue> LoadItems(InstrumentType instrumentType, Dictionary<int, string> itemValues)
        {
            if (instrumentType == null)
                throw new ArgumentNullException(nameof(instrumentType));

            var metadata = LoadItems(instrumentType);

            return itemValues.Select(iv => new ItemValue(metadata.GetItem(iv.Key), iv.Value));
        }

        public static IEnumerable<ItemMetadata> LoadItems(InstrumentType type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (ItemFileCache.ContainsKey(type))
                return ItemFileCache[type];

            return new List<ItemMetadata>();
        }
    }
}