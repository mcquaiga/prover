using System.Collections.Generic;
using Prover.CommProtocol.Common.Items;
using System;
using System.Reactive.Subjects;
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

        public string CommClientType { get; set; }
        public Func<ICommPort, ISubject<string>, EvcCommunicationClient> ClientFactory { get; set; }

        public virtual HashSet<ItemMetadata> Items { get; }

        public bool? CanUseIrDaPort { get; set; }
        public abstract void LoadItemsInformation();     

        public int? MaxBaudRate { get; set; }

        public IEnumerable<ItemMetadata> ItemsMetadata { get; set; }
    }
}