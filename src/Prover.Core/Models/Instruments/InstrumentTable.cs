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
    public abstract class ProverTable : EntityWithId
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
            {
                var id = (this as Instrument)?.Type;
                if (!id.HasValue) throw new NullReferenceException("Instrument ID could not be found.");
                InstrumentType = HoneywellInstrumentTypes.GetById(id.Value);
            }

            if (InstrumentType == null)
                throw new NullReferenceException(nameof(InstrumentType));
            if (string.IsNullOrEmpty(_instrumentData))
                throw new NullReferenceException(nameof(_instrumentData));

            var itemValues = JsonConvert.DeserializeObject<Dictionary<int, string>>(_instrumentData);
            Items = ItemHelpers.LoadItems(InstrumentType, itemValues).ToList();
        }
    }
}