namespace Prover.Core.VerificationTests
{
    using Prover.CommProtocol.Common;
    using Prover.CommProtocol.Common.IO;
    using Prover.Core.Models.Instruments;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    #region Interfaces

    /// <summary>
    /// Defines the <see cref="ITestRunManager" />
    /// </summary>
    public interface ITestRunManager : IDisposable
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

        #endregion

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
        /// <param name="instrumentType">The instrumentType<see cref="InstrumentType"/></param>
        /// <param name="commPort">The commPort<see cref="CommPort"/></param>
        /// <returns>The <see cref="Task"/></returns>
        Task InitializeTest(InstrumentType instrumentType, CommPort commPort);

        Task InitializeTest(InstrumentType instrumentType, CommPort commPort, Dictionary<int, decimal> setItemValues);

        /// <summary>
        /// The RunTest
        /// </summary>
        /// <param name="level">The level<see cref="int"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        Task RunTest(int level, CancellationToken ct = new CancellationToken());

        /// <summary>
        /// The RunVerifier
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        Task RunVerifier();

        /// <summary>
        /// The SaveAsync
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        Task SaveAsync();

        #endregion
    }

    #endregion
}
