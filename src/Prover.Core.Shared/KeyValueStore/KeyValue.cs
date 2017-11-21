using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Core.Shared.Domain;

namespace Prover.Core.Shared.KeyValueStore
{
    public class KeyValue : GenericEntity<string>
    {
        public string Value { get; set; }
    }
}
