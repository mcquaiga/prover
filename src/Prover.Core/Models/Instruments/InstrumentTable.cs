using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public abstract class ProverTable : Entity
    {
        private string _instrumentData;

        public string InstrumentData
        {
            get => Items.Serialize();
            set => _instrumentData = value;
        }

        [NotMapped]
        public ICollection<ItemValue> Items { get; set; }

        [NotMapped]
        public virtual InstrumentType InstrumentType { get; set; }

        public override void OnInitializing()
        {
            if (this is Instrument)
                InstrumentType = HoneywellInstrumentTypes.GetAll()
                    .FirstOrDefault(i => i.Id == (this as Instrument)?.Type);

            if (string.IsNullOrEmpty(_instrumentData)) return;

            var itemValues = JsonConvert.DeserializeObject<Dictionary<int, string>>(_instrumentData);
            Items = ItemHelpers.LoadItems(InstrumentType, itemValues).ToList();
        }
    }
}