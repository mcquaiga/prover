using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell;
using Prover.CommProtocol.MiHoneywell.Items;
using Prover.Core.Shared.Domain;

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
        public virtual EvcDevice InstrumentType { get; set; }

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