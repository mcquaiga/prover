using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;

namespace Prover.CommProtocol.MiHoneywell.Items
{
    public static class ItemHelpers
    {
        private const string ItemDefinitionsFolder = "ItemDefinitions";

        public static IEnumerable<ItemValue> LoadItems(InstrumentType instrumentType, Dictionary<int, string> itemValues)
        {
            var metadata = LoadItems(instrumentType);

            return itemValues.Select(iv => new ItemValue(metadata.GetItem(iv.Key), iv.Value));
        }

        public static IEnumerable<ItemMetadata> LoadItems(InstrumentType type)
        {
            var path = $@"{Environment.CurrentDirectory}\{ItemDefinitionsFolder}\{type.ItemFilePath}";
            var xDoc = XDocument.Load(path);

            return (from x in xDoc.Descendants("item")
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
                                            ? (decimal?) null
                                            : Convert.ToDecimal(y.Attribute("numericvalue").Value)
                                })
                            .ToList()
                }
            ).ToList();
        }
    }
}