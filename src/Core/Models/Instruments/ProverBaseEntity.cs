using System;
using System.Collections.Generic;
using System.Linq;

namespace Prover.Core.Models.Instruments
{
    public abstract class ProverBaseEntity : EntityWithId
    {
        private string _instrumentData;

        public string InstrumentData
        {
            get => Items.Serialize();
            set => _instrumentData = value;
        }

        [NotMapped]
        public IEnumerable<ItemValue> Items { get; set; }

        [NotMapped]
        public virtual InstrumentType InstrumentType { get; set; }

        public override void OnInitializing()
        {        
            if (InstrumentType == null)
                throw new NullReferenceException(nameof(InstrumentType));
            if (string.IsNullOrEmpty(_instrumentData))
                throw new NullReferenceException(nameof(_instrumentData));

            var itemValues = JsonConvert.DeserializeObject<Dictionary<int, string>>(_instrumentData);
            Items = ItemHelpers.LoadItems(InstrumentType, itemValues).ToList();
        }
    }
}