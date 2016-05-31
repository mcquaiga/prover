using Prover.Core.Models.Instruments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Prover.Core.Models
{
    public class InstrumentItems
    {
        public InstrumentItems()
        {
            Items = new List<ItemDetail>();
        }

        public InstrumentItems(InstrumentType instrumentType)
        {
            Items = ItemHelpers.LoadItems(instrumentType);
        }

        public InstrumentItems(IEnumerable<ItemDetail> items)
        {
            Items = items;
        }

        public IEnumerable<ItemDetail> Items { get; private set; }        

        public ItemDetail GetItem(int itemNumber)
        {
            return Items.FirstOrDefault(i => i.Number == itemNumber);
        }

        public ItemDetail GetItem(string code)
        {
            return Items.FirstOrDefault(i => i.Code == code);
        }

        public int GetItemNumberByCode(string itemCode)
        {
            return this.Items.FirstOrDefault(i => i.Code == itemCode).Number;
        }
    }

    public class ItemDetail
    {
        public int Number { get; set; }
        public string Code { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }

        public bool? IsAlarm { get; set; }
        public bool? IsPressure { get; set; }
        public bool? IsPressureTest { get; set; }
        public bool? IsTemperature { get; set; }
        public bool? IsTemperatureTest { get; set; }
        public bool? IsVolume { get; set; }
        public bool? IsVolumeTest { get; set; }
        public bool? IsSuperFactor { get; set; }

        public decimal GetNumericValue(Dictionary<int, string> itemValues)
        {
            var value = itemValues.FirstOrDefault(i => i.Key == Number).Value;
            return GetNumericValue(value);
        }

        public decimal GetNumericValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new NullReferenceException(string.Format("No value was found for Item Number #{0}.", Number));

            var decValue = Convert.ToDecimal(value);
            if (ItemDescriptions.Any())
            {
                var intValue = Convert.ToInt32(decValue);
                return GetItemDescription(intValue)?.Value ?? intValue;
            }

            return decValue;
        }

        public string GetDescriptionValue(Dictionary<int, string> itemValues)
        {
            var value = itemValues.FirstOrDefault(i => i.Key == Number).Value;
            return GetDescriptionValue(value);
        }

        public string GetDescriptionValue(string value)
        {
            var intValue = Convert.ToInt32(value);
            if (ItemDescriptions != null)
            {
                return GetItemDescription(intValue)?.Description;
            }

            throw new KeyNotFoundException($"No description found for #{Number} - {Code}");
        }

        private ItemDescription GetItemDescription(int intValue)
        {
            return ItemDescriptions.FirstOrDefault(x => x.Id == intValue);
        }

        [NotMapped]
        public IEnumerable<ItemDescription> ItemDescriptions { get; set; }

        public class ItemDescription
        {
            public int Id { get; set; } //Maps to the Id that the instrument uses
            public string Description { get; set; } //Human displayed description
            public decimal? Value { get; set; } // Numeric value used for calculations, etc.
        }
    }

}
