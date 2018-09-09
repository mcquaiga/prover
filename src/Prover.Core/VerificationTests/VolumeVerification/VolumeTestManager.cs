namespace Prover.Core.VerificationTests.VolumeVerification
{
    using Caliburn.Micro;
    using MccDaq;
    using NLog;
    using Prover.CommProtocol.Common;
    using Prover.Core.ExternalDevices.DInOutBoards;
    using Prover.Core.Models.Instruments;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using LogManager = NLog.LogManager;

    /// <summary>
    /// Defines the <see cref="VolumeTestManager" />
    /// </summary>
    public abstract class VolumeTestManager : IDisposable
    {
        #region Fields

        /// <summary>
        /// Defines the EventAggreator
        /// </summary>
        protected IEventAggregator EventAggreator;

        /// <summary>
        /// Defines the FirstPortAInputBoard
        /// </summary>
        protected IDInOutBoard FirstPortAInputBoard;

        /// <summary>
        /// Defines the FirstPortBInputBoard
        /// </summary>
        protected IDInOutBoard FirstPortBInputBoard;

        /// <summary>
        /// Defines the IsFirstVolumeTest
        /// </summary>
        protected bool IsFirstVolumeTest = true;

        /// <summary>
        /// Defines the Log
        /// </summary>
        protected Logger Log;

        /// <summary>
        /// Defines the RequestStopTest
        /// </summary>
        protected bool RequestStopTest;

        /// <summary>
        /// Defines the TestCancellationToken
        /// </summary>
        protected CancellationTokenSource TestCancellationToken;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VolumeTestManager"/> class.
        /// </summary>
        /// <param name="eventAggregator">The eventAggregator<see cref="IEventAggregator"/></param>
        protected VolumeTestManager(IEventAggregator eventAggregator)
        {
            Log = LogManager.GetCurrentClassLogger();
            EventAggreator = eventAggregator;

            //DaqService = daqService;
            FirstPortAInputBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortA, 0);
            FirstPortBInputBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortB, 1);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether RunningTest
        /// </summary>
        public bool RunningTest { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The CompleteTest
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <param name="testActionsManager">The testActionsManager<see cref="ITestActionsManager"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <param name="readTach">The readTach<see cref="bool"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public abstract Task CompleteTest(EvcCommunicationClient commClient, VolumeTest volumeTest,
           ITestActionsManager testActionsManager, CancellationToken ct, bool readTach = true);

        /// <summary>
        /// The Dispose
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// The InitializeTest
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <param name="testActionsManager">The testActionsManager<see cref="ITestActionsManager"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public abstract Task InitializeTest(EvcCommunicationClient commClient, VolumeTest volumeTest, ITestActionsManager testActionsManager);

        /// <summary>
        /// The RunTest
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <param name="testActionsManager">The testActionsManager<see cref="ITestActionsManager"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public virtual async Task RunFullVolumeTest(EvcCommunicationClient commClient, VolumeTest volumeTest, ITestActionsManager testActionsManager,
            CancellationToken ct)
        {
            try
            {
                RunningTest = true;

                await Task.Run(async () =>
                {
                    Log.Info("Volume test started!");

                    await RunSyncTest(commClient, volumeTest, ct);
                    ct.ThrowIfCancellationRequested();

                    await InitializeTest(commClient, volumeTest, testActionsManager);

                    await StartRunningVolumeTest(volumeTest, ct);
                    ct.ThrowIfCancellationRequested();

                    await CompleteTest(commClient, volumeTest, testActionsManager, ct);

                    Log.Info("Volume test finished!");
                }, ct);
            }
            catch (OperationCanceledException ex)
            {
                Log.Info("volume test cancellation requested.");
                throw;
            }
            finally
            {
                RunningTest = false;
            }
        }

        /// <summary>
        /// The RunSyncTest
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        protected abstract Task RunSyncTest(EvcCommunicationClient commClient, VolumeTest volumeTest, CancellationToken ct);

        /// <summary>
        /// The StartRunningVolumeTest
        /// </summary>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        protected abstract Task StartRunningVolumeTest(VolumeTest volumeTest, CancellationToken ct);

        #endregion
    }
}
