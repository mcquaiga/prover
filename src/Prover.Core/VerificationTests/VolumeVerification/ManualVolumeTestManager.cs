namespace Prover.Core.VerificationTests.VolumeVerification
{
    using Caliburn.Micro;
    using Prover.CommProtocol.Common;
    using Prover.Core.Models.Instruments;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="ManualVolumeTestManager" />
    /// </summary>
    public sealed class ManualVolumeTestManager : VolumeTestManager
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ManualVolumeTestManager"/> class.
        /// </summary>
        /// <param name="eventAggregator">The eventAggregator<see cref="IEventAggregator"/></param>
        public ManualVolumeTestManager(IEventAggregator eventAggregator) : base(eventAggregator)
        {
        }

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
        public override async Task CompleteTest(EvcCommunicationClient commClient, VolumeTest volumeTest, ITestActionsManager testActionsManager, CancellationToken ct, bool readTach = true)
        {
            try
            {
                await commClient.Connect();

                volumeTest.AfterTestItems = await commClient.GetVolumeItems();

                if (volumeTest.VerificationTest.FrequencyTest != null)
                {
                    volumeTest.VerificationTest.FrequencyTest.PostTestItemValues = await commClient.GetFrequencyItems();
                }

                await testActionsManager?.RunVolumeTestCompleteActions(commClient, volumeTest.Instrument);
            }
            finally
            {
                await commClient.Disconnect();
            }
        }

        /// <summary>
        /// The Dispose
        /// </summary>
        public override void Dispose()
        {
        }

        /// <summary>
        /// The InitializeTest
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <param name="testActionsManager">The testActionsManager<see cref="ITestActionsManager"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public override async Task InitializeTest(EvcCommunicationClient commClient, VolumeTest volumeTest, ITestActionsManager testActionsManager)
        {
            await commClient.Connect();

            await testActionsManager.RunVolumeTestInitActions(commClient, volumeTest.Instrument);

            volumeTest.Items = await commClient.GetVolumeItems();

            if (volumeTest.VerificationTest.FrequencyTest != null)
            {
                volumeTest.VerificationTest.FrequencyTest.PreTestItemValues = await commClient.GetFrequencyItems();
            }

            await commClient.Disconnect();
        }

        /// <summary>
        /// The RunTest
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <param name="testActionsManager">The testActionsManager<see cref="ITestActionsManager"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public override async Task RunFullVolumeTest(EvcCommunicationClient commClient, VolumeTest volumeTest, ITestActionsManager testActionsManager, CancellationToken ct)
        {
            return;
        }

        /// <summary>
        /// The RunSyncTest
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        protected override async Task RunSyncTest(EvcCommunicationClient commClient, VolumeTest volumeTest, CancellationToken ct)
        {
            return;
        }

        /// <summary>
        /// The StartRunningVolumeTest
        /// </summary>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        protected override async Task StartRunningVolumeTest(VolumeTest volumeTest, CancellationToken ct)
        {
            return;
        }

        #endregion
    }
}
