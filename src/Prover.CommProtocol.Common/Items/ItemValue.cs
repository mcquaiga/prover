using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.CommProtocol.Common.Items
{
    public class ItemValue
    {
        public ItemValue(ItemMetadata itemMetadata, string value)
        {
            this.ItemMetadata = itemMetadata;
            this.Value = value;
        }

        public string Value { get; set; }

        public ItemMetadata ItemMetadata { get; set; }
    }
}
