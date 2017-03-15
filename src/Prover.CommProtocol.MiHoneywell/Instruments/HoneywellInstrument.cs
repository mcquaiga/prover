using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Instruments;
using Prover.CommProtocol.Common.Items;
using Prover.Shared.Enums;

namespace Prover.CommProtocol.MiHoneywell.Instruments
{
    public static class InstrumentsHoneywell
    {
        public static MiniAtInstrument MiniAtInstrument 
            => new MiniAtInstrument();
        
        public static MiniMaxInstrument MiniMaxInstrument
            => new MiniMaxInstrument();
    }

    public class MiniAtInstrument : HoneywellInstrument
    {
        public MiniAtInstrument() 
            : base(3, 33333, "Mini-AT", "MiniATItems.xml")
        {
        }
    }

    public class MiniMaxInstrument : HoneywellInstrument
    {
        public MiniMaxInstrument() 
            : base(4, 33333, "Mini-Max", "MiniMaxItems.xml")
        {
        }
    }

    public abstract class HoneywellInstrument : Instrument
    {
        protected HoneywellInstrument(int id, int accessCode, string name, string itemFilePath) : base(id, accessCode, name, itemFilePath)
        {
            ItemDefinitions = ItemFileLoader.LoadItems(this);
        }

        public override ItemValue GetItemValue(string itemCode, Dictionary<string, string> itemValues)
        {
            if (string.IsNullOrEmpty(itemCode)) return null;

            var itemInfo = ItemDefinitions.FirstOrDefault(i => i.Code == itemCode);
            if (itemInfo == null) return null;

            var key = itemInfo.Number.ToString();
            if (itemValues.ContainsKey(key))
            {
                return new ItemValue(itemInfo, itemValues[key]);
            }

            return new ItemValue(itemInfo, string.Empty);
        }

        public override EvcCorrectorType CorrectorType(Dictionary<string, string> itemValues)
        {
            var live = "Live";

            var pressureFixed = GetItemValue(ItemCodes.Pressure.FixedFactor, itemValues).Description == live;
            var tempFixed = GetItemValue(ItemCodes.Temperature.FixedFactor, itemValues).Description == live;
            var superFixed = GetItemValue(ItemCodes.Super.FixedFactor, itemValues).Description == live;

            if (pressureFixed && tempFixed && superFixed)
                return EvcCorrectorType.PTZ;

            if (pressureFixed)
                return EvcCorrectorType.P;

            if (tempFixed)
                return EvcCorrectorType.T;

            throw new NotSupportedException("Could not determine the corrector type.");
        }
    }

    internal static class ItemFileLoader
    {
        private const string ItemDefinitionsFolder = "ItemDefinitions";
        private static readonly Dictionary<string, IEnumerable<ItemMetadata>> ItemFileCache = new Dictionary<string, IEnumerable<ItemMetadata>>();

        public static IEnumerable<ItemMetadata> LoadItems(HoneywellInstrument instrument)
        {
            var cacheKey = instrument.Name;

            if (ItemFileCache.ContainsKey(cacheKey)) return ItemFileCache[cacheKey];

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

            ItemFileCache.Add(type, items);

            return items;
        }
    }
}
