using System;
using System.Collections.Generic;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;

namespace Prover.CommProtocol.Common
{
    public abstract class InstrumentType
    {        
        public int AccessCode { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public string ItemFilePath { get; set; }
        public Func<CommPort, EvcCommunicationClient> ClientFactory { get; set; }

        public virtual HashSet<ItemMetadata> Items { get; }

        public abstract void LoadItemsInformation();     
    }
}