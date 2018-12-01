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
        public static ITestRunManager MiniAtTestManager { get; set; }

        /// <summary>
        /// Gets the TocTestManager
        /// </summary>
        public static ITestRunManager TocTestManager { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// The CreateNextTestRun
        /// </summary>
        /// <param name="instrumentType">The instrumentType<see cref="InstrumentType"/></param>
        /// <param name="commPort">The commPort<see cref="CommPort"/></param>
        /// <param name="statusAction">The statusAction<see cref="Action{string}"/></param>
        /// <returns>The <see cref="Task{IQaRunTestManager}"/></returns>
        public static async Task<ITestRunManager> CreateNextTestRun(InstrumentType instrumentType, CommPort commPort, Action<string> statusAction = null)
        {
            if (instrumentType == Instruments.Toc)
            {
                TocTestManager = (ITestRunManager)Locator.Current.GetService<ITestRunManager>();
                TocTestManager.TestStatus.Subscribe(statusAction);

                // Item 182 (Input Vol to Corrector)  needs to be changed to value 2 ( 
                var setItemValues = new Dictionary<int, decimal>()
                {
                    {182, 1},
                    {855, 1}
                };

                await TocTestManager.InitializeTest(instrumentType, commPort, setItemValues);

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
        public static async Task<ITestRunManager> CreateTestRun(InstrumentType instrumentType, CommPort commPort, Action<string> statusAction = null)
        {
            var qaTestRunManager = (ITestRunManager)Locator.Current.GetService<ITestRunManager>();
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
        /// <param name="qaTestRunManager">The qaTestRunManager<see cref="ITestRunManager"/></param>
        /// <returns>The <see cref="Task{IQaRunTestManager}"/></returns>
        private static async Task<ITestRunManager> TocTestRun(InstrumentType instrumentType, CommPort commPort, ITestRunManager qaTestRunManager)
        {
            MiniAtTestManager = qaTestRunManager;
            instrumentType = Instruments.MiniAt;

            var setItemValues = new Dictionary<int, decimal>()
            {
                {182, 0},
                {855, 0}
            };

            await qaTestRunManager.InitializeTest(instrumentType, commPort, setItemValues);           

            return qaTestRunManager;
        }

        #endregion
    }
}
