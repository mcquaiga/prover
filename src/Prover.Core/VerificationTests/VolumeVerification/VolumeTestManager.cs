namespace Prover.Core.VerificationTests.VolumeVerification
{
    using Caliburn.Micro;
    using NLog;
    using Prover.CommProtocol.Common;
    using Prover.Core.ExternalDevices.DInOutBoards;
    using Prover.Core.Models.Instruments;
    using Prover.Core.Settings;
    using System;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading;
    using System.Threading.Tasks;
    using LogManager = NLog.LogManager;

    #region Enums

    public enum VolumeTestSteps
    {
        PreTest,
        RunningSyncTest,
        ExecutingTest,
        PostTest
    }

    #endregion

    #region Interfaces

    /// <summary>
    /// Defines the <see cref="IPulseInputService" />
    /// </summary>
    public interface IPulseInputService
    {
    }

    #endregion

    /// <summary>
    /// Defines the <see cref="VolumeTestManager" />
    /// </summary>
    public abstract class VolumeTestManager : IDisposable
    {
        #region Fields

        /// <summary>
        /// Defines the Status
        /// </summary>
        protected readonly Subject<string> Status = new Subject<string>();

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
        /// Defines the Log
        /// </summary>
        protected Logger Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VolumeTestManager"/> class.
        /// </summary>
        /// <param name="eventAggregator">The eventAggregator<see cref="IEventAggregator"/></param>
        /// <param name="settingsService">The settingsService<see cref="ISettingsService"/></param>
        protected VolumeTestManager(IEventAggregator eventAggregator, ISettingsService settingsService)
        {

            SettingsService = settingsService;
            EventAggreator = eventAggregator;           
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether RunningTest
        /// </summary>
        public bool RunningTest { get; set; }

        /// <summary>
        /// Gets the SettingsService
        /// </summary>
        public ISettingsService SettingsService { get; }

        /// <summary>
        /// Gets the StatusMessage
        /// </summary>
        public IObservable<string> StatusMessage => Status.AsObservable();

        /// <summary>
        /// Gets or sets the VolumeTest
        /// </summary>
        public VolumeTest VolumeTest { get; set; }

        /// <summary>
        /// Gets or sets the CommClient
        /// </summary>
        protected EvcCommunicationClient CommClient { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The CompleteTest
        /// </summary>
        /// <param name="testActionsManager">The testActionsManager<see cref="ITestActionsManager"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public abstract Task CompleteTest(ITestActionsManager testActionsManager, CancellationToken ct);

        /// <summary>
        /// The Dispose
        /// </summary>
        public virtual void Dispose()
        {
            Status?.Dispose();
        }

        /// <summary>
        /// The ExecuteSyncTest
        /// </summary>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public abstract Task ExecuteSyncTest(CancellationToken ct);

        /// <summary>
        /// The PreTest
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <param name="testActionsManager">The testActionsManager<see cref="ITestActionsManager"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public abstract Task PreTest(EvcCommunicationClient commClient, VolumeTest volumeTest, ITestActionsManager testActionsManager, CancellationToken ct);

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
                CommClient = commClient;
                VolumeTest = volumeTest;

                Log.Info("Volume test started!");

                commClient.Status.Subscribe(Status);
                //TODO: Add setting to skip
                if (SettingsService.TestSettings.RunVolumeSyncTest)
                    await ExecuteSyncTest(ct);

                ct.ThrowIfCancellationRequested();

                await PreTest(commClient, volumeTest, testActionsManager, ct);

                await RunTest(ct);
                ct.ThrowIfCancellationRequested();

                await CompleteTest(testActionsManager, ct);

                Log.Info("Volume test finished!");

            }
            catch (OperationCanceledException)
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
        /// The RunTest
        /// </summary>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public abstract Task RunTest(CancellationToken ct);

        /// <summary>
        /// The ResetPulseCounts
        /// </summary>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        protected static void ResetPulseCounts(VolumeTest volumeTest)
        {
            volumeTest.PulseACount = 0;
            volumeTest.PulseBCount = 0;
        }

        #endregion
    }
}
