using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Prover.CommProtocol.Common.Items;

namespace Prover.CommProtocol.MiHoneywell.Instruments
{
    internal static class HoneywellItemDefinitions
    {
        private const string ItemDefinitionsFolder = "ItemDefinitions";

        private static readonly Dictionary<string, IEnumerable<ItemMetadata>> ItemFileCache =
            new Dictionary<string, IEnumerable<ItemMetadata>>();

        internal static IEnumerable<ItemMetadata> Load(HoneywellInstrument instrument)
        {
            var cacheKey = instrument.Name;

            if (ItemFileCache.ContainsKey(cacheKey))
                return ItemFileCache[cacheKey];

            var items = LoadFromFile(instrument).ToList();

            ItemFileCache.Add(cacheKey, items);

            return items;
        }

        internal static IEnumerable<ItemMetadata> LoadFromFile(HoneywellInstrument instrument)
        {
            var path = $@"{Environment.CurrentDirectory}\{ItemDefinitionsFolder}\{instrument.ItemFilePath}";
            var xDoc = XDocument.Load(path);

            var items = (from x in xDoc.Descendants("item")
                select new ItemMetadata
                {
                    Number = Convert.ToInt32(x.Attribute("number").Value),
                    Code = x.Attribute("code") == null ? "" : x.Attribute("code").Value,
                    ShortDescription =
                        x.Attribute("shortDescription") == null ? "" : x.Attribute("shortDescription").Value,
                    LongDescription = x.Attribute("description") == null ? "" : x.Attribute("description").Value,
                    IsAlarm = x.Attribute("isAlarm") != null && Convert.ToBoolean(x.Attribute("isAlarm").Value),
                    IsPressure =
                        x.Attribute("isPressure") != null && Convert.ToBoolean(x.Attribute("isPressure").Value),
                    IsPressureTest =
                        x.Attribute("isPressureTest") != null &&
                        Convert.ToBoolean(x.Attribute("isPressureTest").Value),
                    IsTemperature =
                        x.Attribute("isTemperature") != null && Convert.ToBoolean(x.Attribute("isTemperature").Value),
                    IsTemperatureTest =
                        x.Attribute("isTemperatureTest") != null &&
                        Convert.ToBoolean(x.Attribute("isTemperatureTest").Value),
                    IsVolume = x.Attribute("isVolume") != null && Convert.ToBoolean(x.Attribute("isVolume").Value),
                    IsVolumeTest =
                        x.Attribute("isVolumeTest") != null && Convert.ToBoolean(x.Attribute("isVolumeTest").Value),
                    IsSuperFactor = x.Attribute("isSuper") != null && Convert.ToBoolean(x.Attribute("isSuper").Value),
                    ItemDescriptions =
                        (from y in x.Descendants("value")
                            select new ItemMetadata.ItemDescription
                            {
                                Id = Convert.ToInt32(y.Attribute("id").Value),
                                Description = y.Attribute("description").Value,
                                Value =
                                    y.Attribute("numericvalue") == null
                                        ? (decimal?) null
                                        : Convert.ToDecimal(y.Attribute("numericvalue").Value)
                            })
                        .ToList()
                }
            ).ToList();

            

            return items;
        }
    }
}