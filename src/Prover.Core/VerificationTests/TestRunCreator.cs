namespace Prover.Core.VerificationTests
{
    using Caliburn.Micro;
    using Prover.CommProtocol.Common;
    using Prover.CommProtocol.Common.IO;
    using Prover.CommProtocol.Common.Models.Instrument;
    using Prover.CommProtocol.MiHoneywell;
    using Prover.Core.Models.Clients;
    using Prover.Core.Models.Instruments;
    using Prover.Core.Settings;
    using Prover.Core.VerificationTests.TestActions;
    using System;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="TestRunCreator" />
    /// </summary>
    public static class TestRunCreator
    {
        #region Properties

        /// <summary>
        /// Gets or sets the MiniAtTestManager
        /// </summary>
        public static IQaRunTestManager MiniAtTestManager { get; set; }

        /// <summary>
        /// Gets the TocTestManager
        /// </summary>
        public static IQaRunTestManager TocTestManager { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// The CreateNextTestRun
        /// </summary>
        /// <param name="instrumentType">The instrumentType<see cref="EvcDevice"/></param>
        /// <param name="commPort">The commPort<see cref="CommPort"/></param>
        /// <param name="statusAction">The statusAction<see cref="Action{string}"/></param>
        /// <returns>The <see cref="Task{IQaRunTestManager}"/></returns>
        public static async Task<IQaRunTestManager> CreateNextTestRun(IEvcDevice instrumentType, ICommPort commPort, ISettingsService settingsService,
            Client client = null, CancellationToken ct = new CancellationToken(), Action<string> statusAction = null)
        {
            if (instrumentType == HoneywellInstrumentTypes.Toc)
            {
                TocTestManager = IoC.Get<IQaRunTestManager>();
                TocTestManager.Status.Subscribe(statusAction);
                            
                Func<EvcCommunicationClient, Instrument, Task> testFunc = async (comm, instrument) =>
                {
                    await comm.SetItemValue(182, 1);
                    await comm.SetItemValue(855, 1);
                };

                IoC.Get<ITestActionsManager>().RegisterAction(VerificationStep.PreVerification, testFunc);
                await TocTestManager.InitializeTest(instrumentType, commPort, settingsService, ct);
                IoC.Get<ITestActionsManager>().UnregisterActions(VerificationStep.PreVerification, testFunc);


                MiniAtTestManager.Instrument.LinkedTest = TocTestManager.Instrument;
                MiniAtTestManager.Instrument.LinkedTestId = TocTestManager.Instrument.Id;

                return TocTestManager;
            }

            return null;
        }

        /// <summary>
        /// The CreateTestRun
        /// </summary>
        /// <param name="instrumentType">The instrumentType<see cref="EvcDevice"/></param>
        /// <param name="commPort">The commPort<see cref="CommPort"/></param>
        /// <param name="statusAction">The statusAction<see cref="Action{string}"/></param>
        /// <returns>The <see cref="Task{IQaRunTestManager}"/></returns>
        public static async Task<IQaRunTestManager> CreateTestRun(IEvcDevice instrumentType, ICommPort commPort, ISettingsService settingsService,
            Client client = null, CancellationToken ct = new CancellationToken(), Action<string> statusAction = null)
        {
            var qaTestRunManager = IoC.Get<IQaRunTestManager>();
            qaTestRunManager.Status.Subscribe(statusAction);

            if (instrumentType == HoneywellInstrumentTypes.Toc)
            {
                return await TocTestRun(qaTestRunManager, instrumentType, commPort, settingsService, client, ct);
            }

            await qaTestRunManager.InitializeTest(instrumentType, commPort, settingsService, ct, client);
            return qaTestRunManager;
        }

        /// <summary>
        /// The CreateTocTestRun
        /// </summary>
        /// <param name="instrumentType">The instrumentType<see cref="EvcDevice"/></param>
        /// <param name="commPort">The commPort<see cref="CommPort"/></param>
        /// <param name="qaTestRunManager">The qaTestRunManager<see cref="IQaRunTestManager"/></param>
        /// <returns>The <see cref="Task{IQaRunTestManager}"/></returns>
        private static async Task<IQaRunTestManager> TocTestRun(IQaRunTestManager qaTestRunManager, IEvcDevice instrumentType, ICommPort commPort,
            ISettingsService settingsService, Client client = null, CancellationToken ct = new CancellationToken(), Action<string> statusAction = null)
        {
            MiniAtTestManager = qaTestRunManager;
            instrumentType = HoneywellInstrumentTypes.MiniAt;

            Func<EvcCommunicationClient, Instrument, Task> testFunc = async (comm, instrument) =>
                {
                    await comm.SetItemValue(182, 0);
                    await comm.SetItemValue(855, 0);
                };

            IoC.Get<ITestActionsManager>().RegisterAction(VerificationStep.PreVerification, testFunc);
            await qaTestRunManager.InitializeTest(instrumentType, commPort, settingsService, ct, client);           
            IoC.Get<ITestActionsManager>().UnregisterActions(VerificationStep.PreVerification, testFunc);

            return qaTestRunManager;
        }

        #endregion
    }
}
