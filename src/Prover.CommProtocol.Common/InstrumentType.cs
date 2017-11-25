using System.Collections.Generic;
using Prover.CommProtocol.Common.Items;
using System;
using Prover.CommProtocol.Common.IO;

namespace Prover.CommProtocol.Common
{
    public class InstrumentType
    {
        public int AccessCode { get; set; }
        public string Name { get; set; }
        public int Id { get; set; }
        public string ItemFilePath { get; set; }
        public Func<CommPort, EvcCommunicationClient> ClientFactory { get; set; }
        public bool? CanUseIrDaPort { get; set; }

        public int? MaxBaudRate { get; set; }

        public IEnumerable<ItemMetadata> ItemsMetadata { get; set; }
    }
}