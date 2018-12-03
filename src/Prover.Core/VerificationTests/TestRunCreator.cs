namespace Prover.Core.VerificationTests
{
    using Caliburn.Micro;
    using NLog;
    using Prover.CommProtocol.Common;
    using Prover.CommProtocol.Common.IO;
    using Prover.CommProtocol.MiHoneywell;
    using Prover.Core.DriveTypes;
    using Prover.Core.ExternalIntegrations.Validators;
    using Prover.Core.Models.Instruments;
    using Prover.Core.Storage;
    using Prover.Core.VerificationTests.VolumeVerification;
    using Splat;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Threading;
    using System.Threading.Tasks;
    using LogManager = NLog.LogManager;

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
        /// <param name="instrumentType">The instrumentType<see cref="InstrumentType"/></param>
        /// <param name="commPort">The commPort<see cref="CommPort"/></param>
        /// <param name="statusAction">The statusAction<see cref="Action{string}"/></param>
        /// <returns>The <see cref="Task{IQaRunTestManager}"/></returns>
        public static async Task<IQaRunTestManager> CreateNextTestRun(InstrumentType instrumentType, CommPort commPort, Action<string> statusAction = null)
        {
            if (instrumentType == Instruments.Toc)
            {
                TocTestManager = Locator.Current.GetService<IQaRunTestManager>();
                TocTestManager.TestStatus.Subscribe(statusAction);
                            
                Func<EvcCommunicationClient, Instrument, Task> testFunc = async (comm, instrument) =>
                {
                    await comm.SetItemValue(182, 1);
                    await comm.SetItemValue(855, 1);
                };

                IoC.Get<ITestActionsManager>().RegisterAction(TestActionsManager.TestActionStep.PreVerification, testFunc);

                await TocTestManager.InitializeTest(instrumentType, commPort);
                IoC.Get<ITestActionsManager>().UnregisterActions(TestActionsManager.TestActionStep.PreVerification, testFunc);


                MiniAtTestManager.Instrument.LinkedTest = TocTestManager.Instrument;
                MiniAtTestManager.Instrument.LinkedTestId = TocTestManager.Instrument.Id;

                return TocTestManager;
            }

            return null;
        }

        /// <summary>
        /// The CreateTestRun
        /// </summary>
        /// <param name="instrumentType">The instrumentType<see cref="InstrumentType"/></param>
        /// <param name="commPort">The commPort<see cref="CommPort"/></param>
        /// <param name="statusAction">The statusAction<see cref="Action{string}"/></param>
        /// <returns>The <see cref="Task{IQaRunTestManager}"/></returns>
        public static async Task<IQaRunTestManager> CreateTestRun(InstrumentType instrumentType, CommPort commPort, Action<string> statusAction = null)
        {
            var qaTestRunManager = (IQaRunTestManager)Locator.Current.GetService<IQaRunTestManager>();
            qaTestRunManager.TestStatus.Subscribe(statusAction);

            if (instrumentType == Instruments.Toc)
            {
                return await TocTestRun(instrumentType, commPort, qaTestRunManager);
            }

            await qaTestRunManager.InitializeTest(instrumentType, commPort);
            return qaTestRunManager;
        }

        /// <summary>
        /// The CreateTocTestRun
        /// </summary>
        /// <param name="instrumentType">The instrumentType<see cref="InstrumentType"/></param>
        /// <param name="commPort">The commPort<see cref="CommPort"/></param>
        /// <param name="qaTestRunManager">The qaTestRunManager<see cref="IQaRunTestManager"/></param>
        /// <returns>The <see cref="Task{IQaRunTestManager}"/></returns>
        private static async Task<IQaRunTestManager> TocTestRun(InstrumentType instrumentType, CommPort commPort, IQaRunTestManager qaTestRunManager)
        {
            MiniAtTestManager = qaTestRunManager;
            instrumentType = Instruments.MiniAt;

            Func<EvcCommunicationClient, Instrument, Task> testFunc = async (comm, instrument) =>
                {
                    await comm.SetItemValue(182, 0);
                    await comm.SetItemValue(855, 0);
                };

            IoC.Get<ITestActionsManager>().RegisterAction(TestActionsManager.TestActionStep.PreVerification, testFunc);
            await qaTestRunManager.InitializeTest(instrumentType, commPort);           
            IoC.Get<ITestActionsManager>().UnregisterActions(TestActionsManager.TestActionStep.PreVerification, testFunc);

            return qaTestRunManager;
        }

        #endregion
    }
}
