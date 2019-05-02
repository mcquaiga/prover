namespace Module.EvcVerification.VerificationTests
{
    using Devices.Communications.IO;
    using Module.EvcVerification.VerificationTests.VolumeVerification;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="IQaRunTestManager"/>
    /// </summary>
    public interface IQaRunTestManager : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets the CommunicationClient
        /// </summary>
        EvcCommunicationClient CommunicationClient { get; }

        IEvcDevice Instrument { get; }

        IObservable<string> Status { get; }

        VolumeTestManager VolumeTestManager { get; set; }

        Task DownloadPostVolumeTest(CancellationToken ct);

        Task DownloadPreVolumeTest(CancellationToken ct);

        Task InitializeTest(IEvcDevice instrumentType, ICommPort commPort,
                            CancellationToken ct = new CancellationToken(), Client client = null, bool runVerifiers = true);

        Task RunCorrectionTest(int level, CancellationToken ct = new CancellationToken());

        Task RunVolumeTest(CancellationToken ct);

        Task SaveAsync();
    }

    #endregion
}