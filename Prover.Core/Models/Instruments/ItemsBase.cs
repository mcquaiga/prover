using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Prover.Core.Models.Instruments
{
    public class ItemsBase
    {
        private Dictionary<int, string> _instrumentValues;
        private string _data;

        public Guid Id { get; set; }
        [NotMapped]
        public ICollection<Item> Items { get; set; }
        public string InstrumentData
        {
            get { return JsonConvert.SerializeObject(InstrumentValues); }
            set
            {
                _data = value;
                _instrumentValues = JsonConvert.DeserializeObject<Dictionary<int, string>>(value);
            }
        }

        [NotMapped]
        public Dictionary<int, string> InstrumentValues
        {
            get { return _instrumentValues; }
            set { _instrumentValues = value; }
        }

        public string DescriptionValue(int number) // Is only available when there's something in the list of ItemDescriptions
        {
            var item = Items.FirstOrDefault(x => x.Number == number);
            if (InstrumentValues == null) return null;
            var value = InstrumentValues[number];

            if (item == null || value == null) return "";

            if (item.ItemDescriptions != null)
            {
                var firstOrDefault = item.ItemDescriptions.FirstOrDefault(x => x.Id == Convert.ToInt32(value));
                if (firstOrDefault != null)
                    return firstOrDefault.Description;
            }

            return Convert.ToString(value);
        }
        public double? NumericValue(int number)
        {
            var item = Items.FirstOrDefault(x => x.Number == number);
            if (InstrumentValues == null) return null;
            string value = InstrumentValues[number];

            if (item == null || value == null) return null;

            if (item.ItemDescriptions != null)
            {
                var firstOrDefault = item.ItemDescriptions.FirstOrDefault(x => x.Id == Convert.ToInt32(value));
                if (firstOrDefault != null)
                    return firstOrDefault.Value;
            }

            return Convert.ToDouble(value);
        }


        public class Item
        {
            public int Number { get; set; }
            public string Code { get; set; }
            public string ShortDescription { get; set; }
            public string LongDescription { get; set; }

            public bool? IsAlarm { get; set; }
            public bool? IsPressure { get; set; }
            public bool? IsTemperature { get; set; }
            public bool? IsTemperatureTest { get; set; }
            public bool? IsVolume { get; set; }

            [NotMapped]
            public IEnumerable<ItemDescription> ItemDescriptions { get; set; }

            public static IList<Item> LoadItems(InstrumentType type)
            {
                string _path = "";
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
                        select new Item()
                        {
                            Number = Convert.ToInt32(x.Attribute("number").Value),
                            Code = x.Attribute("code") == null ? "" : x.Attribute("code").Value,
                            ShortDescription = x.Attribute("shortDescription") == null ? "" : x.Attribute("shortDescription").Value,
                            LongDescription = x.Attribute("description") == null ? "" : x.Attribute("description").Value,
                            IsAlarm = x.Attribute("isAlarm") != null && Convert.ToBoolean(x.Attribute("isAlarm").Value),
                            IsPressure = x.Attribute("isPressure") != null && Convert.ToBoolean(x.Attribute("isPressure").Value),
                            IsTemperature = x.Attribute("isTemperature") != null && Convert.ToBoolean(x.Attribute("isTemperature").Value),
                            IsTemperatureTest = x.Attribute("isTemperatureTest") != null && Convert.ToBoolean(x.Attribute("isTemperatureTest").Value),
                            IsVolume = x.Attribute("isVolume") != null && Convert.ToBoolean(x.Attribute("isVolume").Value),
                            ItemDescriptions =
                                (from y in x.Descendants("value")
                                 select new ItemDescription()
                                 {
                                     Id = Convert.ToInt32(y.Attribute("id").Value),
                                     Description = y.Attribute("description").Value,
                                     Value = y.Attribute("numericvalue") == null ? (double?)null : Convert.ToDouble(y.Attribute("numericvalue").Value)
                                 })
                            .ToList()


                        }
                ).ToList();

            }
        }
        public class InstrumentValue
        {
            public int Number { get; set; }
            public double Value { get; set; }
        }
        public class ItemValues
        {
            public Item Item { get; set; }
            public InstrumentValue InstrumentValue { get; set; }

        }
        public class ItemDescription
        {
            public int Id { get; set; } //Maps to the Id that the instrument uses
            public string Description { get; set; } //Human displayed description
            public double? Value { get; set; } // Numeric value used for calculations, etc.
        }
    }
}
