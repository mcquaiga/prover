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
        public InstrumentItems(InstrumentType instrumentType)
        {
            Items = ItemHelpers.LoadItems(instrumentType);
        }

        public InstrumentItems(IEnumerable<ItemDetail> items)
        {
            Items = items;
        }

        public IEnumerable<ItemDetail> Items { get; }        

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

        [NotMapped]
        public Dictionary<int, string> InstrumentValues
        {
            get
            {
                return Items.ToDictionary(i => i.Number, v => v.RawValue);
            }
            set
            {
                foreach (var item in value)
                {
                    Items.FirstOrDefault(i => i.Number == item.Key).RawValue = item.Value;
                }
            }
        }

        public InstrumentItems CopyItemsByFilter(Func<ItemDetail, bool> predicate)
        {
            var toCopy = this.Items.Where(predicate);
            return new InstrumentItems(toCopy);
        }
    }

    public class ItemDetail
    {
        public int Number { get; set; }
        public string RawValue { get; set; }
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

        public bool HasValue()
        {
            return !string.IsNullOrEmpty(RawValue);
        }

        public decimal GetNumericValue()
        {
            if (string.IsNullOrEmpty(RawValue))
                throw new NullReferenceException(string.Format("No value was found for Item Number #{0}.", Number));

            var decValue = Convert.ToDecimal(RawValue);
            if (ItemDescriptions.Any())
            {
                var intValue = Convert.ToInt32(decValue);
                return GetItemDescription(intValue)?.Value ?? intValue;
            }

            return decValue;
        }

        public string GetDescriptionValue()
        {
            var intValue = Convert.ToInt32(RawValue);
            if (ItemDescriptions != null)
            {
                return GetItemDescription(intValue)?.Description;
            }

            throw new KeyNotFoundException(string.Format("No description found for #{0} - {1}", Number, Code));
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

    public static class ItemHelpers
    {
        public static IEnumerable<ItemDetail> LoadItems(InstrumentType type)
        {
            var _path = string.Empty;
            switch (type)
            {
                case InstrumentType.MiniMax:
                    _path = "MiniMaxItems.xml";
                    break;
                case InstrumentType.Ec300:
                    _path = "EC300Items.xml";
                    break;
            }

            var xDoc = XDocument.Load(_path);

            return (from x in xDoc.Descendants("item")
                    select new ItemDetail()
                    {
                        Number = Convert.ToInt32(x.Attribute("number").Value),
                        Code = x.Attribute("code") == null ? "" : x.Attribute("code").Value,
                        ShortDescription = x.Attribute("shortDescription") == null ? "" : x.Attribute("shortDescription").Value,
                        LongDescription = x.Attribute("description") == null ? "" : x.Attribute("description").Value,
                        IsAlarm = x.Attribute("isAlarm") != null && Convert.ToBoolean(x.Attribute("isAlarm").Value),
                        IsPressure = x.Attribute("isPressure") != null && Convert.ToBoolean(x.Attribute("isPressure").Value),
                        IsPressureTest = x.Attribute("isPressureTest") != null && Convert.ToBoolean(x.Attribute("isPressureTest").Value),
                        IsTemperature = x.Attribute("isTemperature") != null && Convert.ToBoolean(x.Attribute("isTemperature").Value),
                        IsTemperatureTest = x.Attribute("isTemperatureTest") != null && Convert.ToBoolean(x.Attribute("isTemperatureTest").Value),
                        IsVolume = x.Attribute("isVolume") != null && Convert.ToBoolean(x.Attribute("isVolume").Value),
                        IsVolumeTest = x.Attribute("isVolumeTest") != null && Convert.ToBoolean(x.Attribute("isVolumeTest").Value),
                        IsSuperFactor = x.Attribute("isSuper") != null && Convert.ToBoolean(x.Attribute("isSuper").Value),
                        ItemDescriptions =
                            (from y in x.Descendants("value")
                             select new ItemDetail.ItemDescription()
                             {
                                 Id = Convert.ToInt32(y.Attribute("id").Value),
                                 Description = y.Attribute("description").Value,
                                 Value = y.Attribute("numericvalue") == null ? (decimal?)null : Convert.ToDecimal(y.Attribute("numericvalue").Value)
                             })
                        .ToList()
                    }
            ).ToList();
        }
    }
}
