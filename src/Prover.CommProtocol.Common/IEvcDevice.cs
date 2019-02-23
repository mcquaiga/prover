using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;

namespace Prover.CommProtocol.Common
{
    public interface IEvcDevice
    {
        int AccessCode { get; set; }
        bool? CanUseIrDaPort { get; set; }
        Func<ICommPort, ISubject<string>, EvcCommunicationClient> ClientFactory { get; set; }
        string CommClientType { get; set; }
        int Id { get; set; }
        string ItemFilePath { get; set; }
        HashSet<ItemMetadata> Items { get; }
        IEnumerable<ItemMetadata> ItemsMetadata { get; set; }
        int? MaxBaudRate { get; set; }
        string Name { get; set; }

        void LoadItemsInformation();
    }
}