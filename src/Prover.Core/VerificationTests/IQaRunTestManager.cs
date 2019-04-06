namespace Prover.Core.VerificationTests
{
    using Prover.CommProtocol.Common;
    using Prover.CommProtocol.Common.IO;
    using Prover.Core.Models.Instruments;
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

        /// <summary>
        /// Gets the Instrument
        /// </summary>
        Instrument Instrument { get; }

        /// <summary>
        /// Gets the TestStatus
        /// </summary>
        IObservable<string> TestStatus { get; }

        Task InitializeTest(InstrumentType instrumentType, ICommPort commPort, ISettingsService testSettings,
            CancellationToken ct = new CancellationToken(), Client client = null, IObserver<string> statusObserver = null);

        #region Methods

        /// <summary>
        /// The DownloadPostVolumeTest
        /// </summary>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        Task DownloadPostVolumeTest(CancellationToken ct = new CancellationToken());

        /// <summary>
        /// The DownloadPreVolumeTest
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        Task DownloadPreVolumeTest();

        /// <summary>
        /// The InitializeTest
        /// </summary>
        /// <param name="instrumentType">The instrumentType<see cref="EvcDevice"/></param>
        /// <param name="commPort">The commPort<see cref="CommPort"/></param>
        /// <returns>The <see cref="Task"/></returns>
        Task InitializeTest(EvcDevice instrumentType, CommPort commPort);       

        /// <summary>
        /// The RunTest
        /// </summary>
        /// <param name="level">The level<see cref="int"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>

        /// <summary>
        /// The RunVerifier
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        /// <summary>
        /// The SaveAsync
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        Task SaveAsync();

        #endregion
    }

    #endregion
}
