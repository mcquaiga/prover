using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Domain.Instrument.Items
{
    public interface IItemsGroup
    {
        Dictionary<string, string> ItemData { get; }
    }
}
