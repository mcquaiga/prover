namespace Prover.Core.VerificationTests
{
    using Prover.CommProtocol.Common;
    using Prover.CommProtocol.Common.IO;
    using Prover.CommProtocol.Common.Models.Instrument;
    using Prover.Core.Models.Clients;
    using Prover.Core.Models.Instruments;
    using Prover.Core.Settings;
    using Prover.Core.VerificationTests.VolumeVerification;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="IQaRunTestManager" />
    /// </summary>
    public interface IQaRunTestManager : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets the CommunicationClient
        /// </summary>
        EvcCommunicationClient CommunicationClient { get; }

        Instrument Instrument { get; }
        IObservable<string> Status { get; }
        VolumeTestManager VolumeTestManager { get; set; }

        Task InitializeTest(IEvcDevice instrumentType, ICommPort commPort, ISettingsService testSettings,
            CancellationToken ct = new CancellationToken(), Client client = null, bool runVerifiers = true);

        Task RunCorrectionTest(int level, CancellationToken ct = new CancellationToken());
        Task RunVolumeTest(CancellationToken ct);
        Task DownloadPreVolumeTest(CancellationToken ct);
        Task DownloadPostVolumeTest(CancellationToken ct);
        Task SaveAsync();
     
    }

    #endregion
}
