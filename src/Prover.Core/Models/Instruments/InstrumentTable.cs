using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell;
using Prover.CommProtocol.MiHoneywell.Items;
using Prover.Core.Extensions;

namespace Prover.Core.Models.Instruments
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        protected BaseEntity()
        {
            Id = Guid.NewGuid();
        }

        public virtual void OnInitializing()
        {

        }
    }

    public abstract class ProverTable : BaseEntity
    {
        private string _instrumentData;
        public string InstrumentData
        {
            get { return Items.Serialize(); }
            set { _instrumentData = value; }
        }

        [NotMapped]
        public IEnumerable<ItemValue> Items { get; set; }

        [NotMapped]
        public virtual InstrumentType InstrumentType { get; set; }

        public override void OnInitializing()
        {
            base.OnInitializing();

            if (this is Instrument)
                this.InstrumentType =
                    CommProtocol.MiHoneywell.Instruments.GetAll().FirstOrDefault(i => i.Id == (this as Instrument).Type);

            if (string.IsNullOrEmpty(_instrumentData)) return;

            var itemValues = JsonConvert.DeserializeObject<Dictionary<int, string>>(_instrumentData);
            Items = ItemHelpers.LoadItems(InstrumentType, itemValues);
        }
    }
}
