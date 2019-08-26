namespace Prover.Core.VerificationTests.VolumeVerification
{
    using Caliburn.Micro;
    using MccDaq;
    using Prover.CommProtocol.Common;
    using Prover.Core.ExternalDevices;
    using Prover.Core.ExternalDevices.DInOutBoards;
    using Prover.Core.Models.Instruments;
    using Prover.Core.Settings;
    using Prover.Core.VerificationTests.Events;
    using PubSub.Extension;
    using System;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="AutoVolumeTestManager" />
    /// </summary>
    public abstract class AutoVolumeTestManager : VolumeTestManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoVolumeTestManager"/> class.
        /// </summary>
        /// <param name="eventAggregator">The eventAggregator<see cref="IEventAggregator"/></param>
        /// <param name="tachComm">The tachComm<see cref="TachometerService"/></param>
        /// <param name="settingsService">The settingsService<see cref="ISettingsService"/></param>
        public AutoVolumeTestManager(IEventAggregator eventAggregator, TachometerService tachComm, ISettingsService settingsService)
            : base(eventAggregator, settingsService)
        {
            TachometerCommunicator = tachComm;

            FirstPortAInputBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortA, 0);
            FirstPortBInputBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortB, 1);

            OutputBoard = DInOutBoardFactory.CreateBoard(0, 0, 0);
        }

        /// <summary>
        /// The PostTest
        /// </summary>
        /// <param name="testActionsManager">The testActionsManager<see cref="ITestActionsManager"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public override async Task CompleteTest(ITestActionsManager testActionsManager, CancellationToken ct)
        {
            Status.OnNext("Completing volume test...");

            ct.ThrowIfCancellationRequested();

            try
            {
                await CheckForResidualPulses(CommClient, ct);

                VolumeTest.AfterTestItems = await CommClient.GetVolumeItems();
                if (VolumeTest.VerificationTest.FrequencyTest != null)
                {
                    VolumeTest.VerificationTest.FrequencyTest.PostTestItemValues = await CommClient.GetFrequencyItems();
                }

                await testActionsManager.ExecuteValidations(TestActions.VerificationStep.PostVolumeVerification, CommClient, VolumeTest.Instrument);
            }
            finally
            {
                await CommClient.Disconnect();
            }

            await GetAppliedInput();
        }

        /// <summary>
        /// The Dispose
        /// </summary>
        public override void Dispose()
        {
            _pulseInputsCancellationTokenSource?.Cancel();
            _pulseInputsCancellationTokenSource?.Dispose();
            TachometerCommunicator?.Dispose();
        }

        /// <summary>
        /// The ExecuteSyncTest
        /// </summary>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public override async Task ExecuteSyncTest(CancellationToken ct)
        {
            await Task.Run(async () =>
            {
                var cts = new CancellationTokenSource();

                try
                {
                    Status.OnNext("Running volume sync test...");

                    await CommClient.Disconnect();

                    ResetPulseCounts(VolumeTest);
                    OutputBoard.StartMotor();

                    var listen = ListenForPulseInputs(VolumeTest, cts.Token);

                    while (VolumeTest.UncPulseCount < 1 && !ct.IsCancellationRequested) { }
                    OutputBoard.StopMotor();

                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
                catch (OperationCanceledException)
                {
                    Status.OnNext("Volume Sync test cancelled.");
                    throw;
                }
                finally
                {
                    cts.Cancel();
                }
            }, ct);

            await CommClient.Disconnect();
        }

        /// <summary>
        /// The PreTest
        /// </summary>
        /// <param name="testActionsManager">The testActionsManager<see cref="ITestActionsManager"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public override async Task PreTest(EvcCommunicationClient commClient, VolumeTest volumeTest, ITestActionsManager testActionsManager, CancellationToken ct)
        {
            await Task.Run(async () =>
            {
                CommClient = commClient;
                VolumeTest = volumeTest;

                await CommClient.Connect(ct);

                await testActionsManager.ExecuteValidations(TestActions.VerificationStep.PreVolumeVerification, CommClient, VolumeTest.Instrument);

                VolumeTest.Items = await CommClient.GetVolumeItems();

                if (VolumeTest.VerificationTest.FrequencyTest != null)
                {
                    VolumeTest.VerificationTest.FrequencyTest.PreTestItemValues = await CommClient.GetFrequencyItems();
                }

                await CommClient.Disconnect();

                if (TachometerCommunicator != null)
                {
                    Status?.OnNext("Resetting Tachometer...");
                    await TachometerCommunicator?.ResetTach();
                }

                ResetPulseCounts(VolumeTest);
            });
        }

        /// <summary>
        /// The ExecutingTest
        /// </summary>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public override async Task RunTest(CancellationToken ct)
        {
            await Task.Run(async () =>
            {
                _pulseInputsCancellationTokenSource = new CancellationTokenSource();
                ResetPulseCounts(VolumeTest);
                var listen = ListenForPulseInputs(VolumeTest, _pulseInputsCancellationTokenSource.Token);

                try
                {
                    ct.ThrowIfCancellationRequested();

                    using (Observable
                            .Interval(TimeSpan.FromMilliseconds(200))
                            .Subscribe(_ => this.Publish(new VolumeTestStatusEvent("Running Volume Test...", VolumeTest))))
                    {
                        OutputBoard?.StartMotor();
                        await WaitForTestComplete(VolumeTest, ct);
                    }

                    ct.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException)
                {
                    _pulseInputsCancellationTokenSource?.Cancel();
                    Log.Info("Cancelling volume test.");
                    throw;
                }
                finally
                {
                    OutputBoard?.StopMotor();
                }
            });
        }

        /// <summary>
        /// Defines the OutputBoard
        /// </summary>
        protected readonly IDInOutBoard OutputBoard;

        /// <summary>
        /// Defines the TachometerCommunicator
        /// </summary>
        protected readonly TachometerService TachometerCommunicator;

        /// <summary>
        /// The WaitForTestComplete
        /// </summary>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        protected abstract Task WaitForTestComplete(VolumeTest volumeTest, CancellationToken ct);

        /// <summary>
        /// Defines the _pulseInputsCancellationTokenSource
        /// </summary>
        private CancellationTokenSource _pulseInputsCancellationTokenSource;

        /// <summary>
        /// The CheckForResidualPulses
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private async Task CheckForResidualPulses(EvcCommunicationClient commClient, CancellationToken ct)
        {
            await Task.Run(() =>
            {
                int pulsesWaiting;
                int lastPulsesWaiting = 0;
                bool keepWaiting = true;

                Status.OnNext("Waiting for residual pulses...");

                using (Observable
                       .Timer(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(15))
                       .Subscribe(async _ =>
                       {
                           pulsesWaiting = 0;

                           if (!commClient.IsConnected)
                           {
                               await commClient.Connect(ct);
                           }

                           foreach (CommProtocol.Common.Items.ItemValue i in await commClient.GetPulseOutputItems())
                           {
                               pulsesWaiting += (int)i.NumericValue;
                           }

                           Status.OnNext($"Waiting for residual pulses...{Environment.NewLine} {pulsesWaiting} total pulses remaining");
                           if (pulsesWaiting > 0 && lastPulsesWaiting != pulsesWaiting)
                           {
                               await commClient.Disconnect();
                               lastPulsesWaiting = pulsesWaiting;
                           }
                           else
                           {
                               keepWaiting = false;
                           }
                       }))
                {
                    while (keepWaiting) { }
                }

                _pulseInputsCancellationTokenSource.Cancel();
            });
        }

        /// <summary>
        /// The GetAppliedInput
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        private async Task GetAppliedInput()
        {
            if (TachometerCommunicator == null)
            {
                return;
            }

            int? result = null;
            int tries = 0;
            do
            {
                try
                {
                    tries++;
                    Log.Debug($"Reading tachometer .... Attempt {tries} of 10");
                    result = await TachometerCommunicator?.ReadTach();
                }
                catch (Exception ex)
                {
                    Log.Error($"An error occured communication with the tachometer: {ex}");
                }
            } while (!result.HasValue && tries < 10);

            Log.Debug($"Applied Input: {result.Value}");

            VolumeTest.AppliedInput = result.Value;
        }

        /// <summary>
        /// The ListenForPulseInputs
        /// </summary>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="CancellationToken"/></returns>
        private async Task ListenForPulseInputs(VolumeTest volumeTest, CancellationToken ct)
        {
            await Task.Run(() =>
            {
                do
                {
                    //TODO: Raise events so the UI can respond
                    volumeTest.PulseACount += FirstPortAInputBoard.ReadInput();
                    volumeTest.PulseBCount += FirstPortBInputBoard.ReadInput();
                }
                while (!ct.IsCancellationRequested);
            });
        }
    }
}