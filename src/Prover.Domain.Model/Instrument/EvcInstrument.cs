using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Prover.Shared.Domain;

namespace Prover.Domain.Model.Instrument
{
    public class EvcInstrument : Entity
    {
        private string _itemsSerialized;
        private Dictionary<string, string> _items;

        public EvcInstrument() : base(Guid.NewGuid())
        {
        }

        public string InstrumentFactory { get; set; }
        public string InstrumentType { get; set; }

        public Dictionary<string, string> Items
        {
            get
            {
                if (_items == null)
                {
                    ItemsSerialized = _itemsSerialized;
                }
                return _items;
            }
            set
            {
                _items = value;
                _itemsSerialized = JsonConvert.SerializeObject(_items);
            }
        }

        [JsonIgnore]
        public string ItemsSerialized
        {
            get
            {
                _itemsSerialized = JsonConvert.SerializeObject(Items);
                return _itemsSerialized;
            }
            private set
            {
                _itemsSerialized = value;
                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                var jsonItems = JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
                _items = jsonItems;
            }
        }
    }
}