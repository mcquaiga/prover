using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.CommProtocol.Common.Items
{
    public static class ItemCodes
    {
        public static class SiteInfo
        {
            public const string SerialNumber = "SERIAL_NUM";
        }

        public static class Pressure
        {
            public const string Base = "BASE_PRESS";
            public const string Atm = "ATM_PRESS";
            public const string Units = "PRESS_UNITS";
            public const string Range = "PRESS_RANGE";
            public const string TransducerType = "TRANSDUCER_TYPE";
            public const string GasPressure = "GAS_PRESS";
            public const string Factor = "PRESS_FACTOR";
            public const string UnsqrFactor = "UNSQRD_SUPER_FACTOR";
        }
    }

    public abstract class ItemMetadata
    {
        public abstract int Number { get; set; }
        public abstract string Code { get; set; }
        public abstract string ShortDescription { get; set; }
        public abstract string LongDescription { get; set; }

        public abstract bool? IsAlarm { get; set; }
        public abstract bool? IsPressure { get; set; }
        public abstract bool? IsPressureTest { get; set; }
        public abstract bool? IsTemperature { get; set; }
        public abstract bool? IsTemperatureTest { get; set; }
        public abstract bool? IsVolume { get; set; }
        public abstract bool? IsVolumeTest { get; set; }
        public abstract bool? IsSuperFactor { get; set; }

        public virtual IEnumerable<ItemDescription> ItemDescriptions { get; set; }

        public class ItemDescription
        {
            public int Id { get; set; } //Maps to the Id that the instrument uses
            public string Description { get; set; } //Human displayed description
            public decimal? Value { get; set; } // Numeric value used for calculations, etc.
        }
    }

    public static class ItemMetadataExtentions
    {
        public static IEnumerable<int> GetAllItemNumbers(this IEnumerable<ItemMetadata> items) => items.Select(i => i.Number);

        /// <summary>
        /// Returns the item number of the first instance found in the collection
        /// </summary>
        /// <param name="items">Items list</param>
        /// <param name="code">Code used to identify the item number - these are set in the ItemDefinition.xml files</param>
        /// <returns></returns>
        public static int GetItem(this IEnumerable<ItemMetadata> items, string code)
        {
            var firstOrDefault = items.FirstOrDefault(i => i.Code == code);

            if (firstOrDefault == null)
                throw new Exception($"Item Code {code} could not be found in item collection.");

            return firstOrDefault.Number;
        }

        public static ItemMetadata GetItem(this IEnumerable<ItemMetadata> items, int itemNumber)
        {
            return items.FirstOrDefault(i => i.Number == itemNumber);
        }

        public static T GetItem<T>(this IEnumerable<ItemMetadata> items, string code) where T : ItemMetadata
        {
            return (T)items.FirstOrDefault(i => i.Code == code);
        }

        public static decimal GetItemValue(this IEnumerable<ItemMetadata> items, Dictionary<int, string> itemValues, string code)
        {
            var result = items.GetItemString(itemValues, code);
            return decimal.Parse(result);
        }

        public static string GetItemString(this IEnumerable<ItemMetadata> items, Dictionary<int, string> itemValues, string code)
        {
            if (itemValues == null || !itemValues.Any()) throw new ArgumentNullException(nameof(itemValues));

            var itemNumber = items.GetItem(code);

            var result = itemValues.FirstOrDefault(x => x.Key == itemNumber).Value;
            if (result == null) throw new KeyNotFoundException($"Item value for {code} and #{itemNumber} not found.");

            return result;
        }       
    }
}
