using global::Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Prover.CommProtocol.Common.Models.Instrument
{
    public interface IEvcDevice
    {
        #region Public Properties

        int AccessCode { get; set; }
        bool? CanUseIrDaPort { get; set; }
        Func<ICommPort, ISubject<string>, EvcCommunicationClient> ClientFactory { get; set; }
        string CommClientType { get; set; }
        int Id { get; set; }
        bool IsHidden { get; set; }
        string ItemFilePath { get; set; }
        HashSet<ItemMetadata> Items { get; }
        IEnumerable<ItemMetadata> ItemsMetadata { get; set; }
        int? MaxBaudRate { get; set; }
        string Name { get; set; }

        #endregion Public Properties

        #region Public Methods

        void LoadItemsInformation();

        #endregion Public Methods
    }
}