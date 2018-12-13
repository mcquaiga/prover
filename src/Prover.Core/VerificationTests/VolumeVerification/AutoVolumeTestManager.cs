namespace Prover.Core.VerificationTests.VolumeVerification
{
    using Caliburn.Micro;
    using Prover.CommProtocol.Common;
    using Prover.Core.Communication;
    using Prover.Core.ExternalDevices.DInOutBoards;
    using Prover.Core.Models.Instruments;
    using System;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="AutoVolumeTestManager" />
    /// </summary>
    public abstract class AutoVolumeTestManager : VolumeTestManager
    {
        #region Fields

        /// <summary>
        /// Defines the _outputBoard
        /// </summary>
        protected readonly IDInOutBoard OutputBoard;

        /// <summary>
        /// Defines the _tachometerCommunicator
        /// </summary>
        protected readonly TachometerService TachometerCommunicator;

        /// <summary>
        /// Defines the _pulseInputsCancellationTokenSource
        /// </summary>
        private CancellationTokenSource _pulseInputsCancellationTokenSource;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoVolumeTestManager"/> class.
        /// </summary>
        /// <param name="eventAggregator">The eventAggregator<see cref="IEventAggregator"/></param>
        /// <param name="tachComm">The tachComm<see cref="TachometerService"/></param>
        public AutoVolumeTestManager(IEventAggregator eventAggregator, TachometerService tachComm)
            : base(eventAggregator)
        {
            TachometerCommunicator = tachComm;
            OutputBoard = DInOutBoardFactory.CreateBoard(0, 0, 0);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The PostTest
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

                await CheckForResidualPulses(commClient, volumeTest, ct);

                volumeTest.AfterTestItems = await commClient.GetVolumeItems();

                await testActionsManager?.RunVolumeTestCompleteActions(commClient, volumeTest.Instrument);
            }
            finally
            {
                await commClient.Disconnect();
            }

            if (readTach) await GetAppliedInput(volumeTest);
        }

        /// <summary>
        /// The Dispose
        /// </summary>
        public override void Dispose()
        {
            TachometerCommunicator.Dispose();
        }

        /// <summary>
        /// The PreTest
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

            await commClient.Disconnect();

            await TachometerCommunicator?.ResetTach();

            ResetPulseCounts(volumeTest);
        }

        /// <summary>
        /// The ExecuteSyncTest
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        protected override async Task RunSyncTest(EvcCommunicationClient commClient, VolumeTest volumeTest, CancellationToken ct)
        {
            try
            {
                await commClient.Disconnect();

                Log.Info("Running volume sync test...");

                await Task.Run(() =>
                {
                    ResetPulseCounts(volumeTest);
                    OutputBoard?.StartMotor();
                    do
                    {
                        volumeTest.PulseACount += FirstPortAInputBoard.ReadInput();
                        volumeTest.PulseBCount += FirstPortBInputBoard.ReadInput();
                    } while (volumeTest.UncPulseCount < 1 && !ct.IsCancellationRequested);
                }, ct);

                ct.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException ex)
            {
                Log.Info("Cancelling volume sync test.");
                throw;
            }
            finally
            {
                OutputBoard.StopMotor();
            }
        }

        /// <summary>
        /// The ExecutingTest
        /// </summary>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        protected override async Task StartRunningVolumeTest(VolumeTest volumeTest, CancellationToken ct)
        {
            try
            {      
                ResetPulseCounts(volumeTest);
                await TachometerCommunicator?.ResetTach();

                OutputBoard?.StartMotor();

                _pulseInputsCancellationTokenSource = new CancellationTokenSource();
                var listen = Task.Run(() => ListenForPulseInputs(volumeTest, _pulseInputsCancellationTokenSource.Token));

                await WaitForTestComplete(volumeTest, ct);                  
               
                ct.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException ex)
            {
                _pulseInputsCancellationTokenSource.Cancel();
                Log.Info("Cancelling volume test.");
                throw;
            }
            finally
            {
                OutputBoard?.StopMotor();
            }
        }

        protected abstract Task WaitForTestComplete(VolumeTest volumeTest, CancellationToken ct);      

        /// <summary>
        /// The CheckForResidualPulses
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private async Task CheckForResidualPulses(EvcCommunicationClient commClient, VolumeTest volumeTest, CancellationToken ct)
        {
            int pulsesWaiting;
            int lastPulsesWaiting = 0;
            bool keepWaiting = true;

            Log.Debug("Waiting for residual pulses...");
            
            do
            {
                pulsesWaiting = 0;

                if (!commClient.IsConnected)
                    await commClient.Connect();

                foreach (var i in await commClient.GetPulseOutputItems())
                {
                    pulsesWaiting += (int)i.NumericValue;
                }

                Log.Debug($"Pulses still waiting - {pulsesWaiting}.");
                if (pulsesWaiting > 0 && lastPulsesWaiting != pulsesWaiting)
                {             
                    await commClient.Disconnect();
                    await Task.Delay(new TimeSpan(0, 0, 20), ct);
                    lastPulsesWaiting = pulsesWaiting;
                }
                else
                {
                    keepWaiting = false;
                }

            } while (keepWaiting && !ct.IsCancellationRequested);

            _pulseInputsCancellationTokenSource.Cancel();
        }

        /// <summary>
        /// The GetAppliedInput
        /// </summary>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private async Task GetAppliedInput(VolumeTest volumeTest)
        {
            if (TachometerCommunicator == null) return;

            int? result = null;
            var tries = 0;
            do
            {
                try
                {
                    tries++;
                    Log.Debug($"Reading tachometer .... Attempt {tries} of 10");
                    result = await TachometerCommunicator.ReadTach();
                }
                catch (Exception ex)
                {
                    Log.Error($"An error occured communication with the tachometer: {ex}");

                }
            } while (!result.HasValue && tries < 10);

            Log.Debug($"Applied Input: {result.Value}");

            volumeTest.AppliedInput = result.Value;
        }

        /// <summary>
        /// The ListenForPulseInputs
        /// </summary>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="CancellationToken"/></returns>
        private CancellationToken ListenForPulseInputs(VolumeTest volumeTest, CancellationToken ct)
        {
            using (Observable
                .Interval(TimeSpan.FromSeconds(5))
                .Subscribe(_ => Log.Debug($"Pulser A = {volumeTest.PulseACount}; Pulser B = {volumeTest.PulseBCount}")))
            {
                do
                {
                    //TODO: Raise events so the UI can respond
                    volumeTest.PulseACount += FirstPortAInputBoard.ReadInput();
                    volumeTest.PulseBCount += FirstPortBInputBoard.ReadInput();
                } 
                while (!ct.IsCancellationRequested);
            }           

            return ct;
        }

        /// <summary>
        /// The ResetPulseCounts
        /// </summary>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        private void ResetPulseCounts(VolumeTest volumeTest)
        {
            FirstPortAInputBoard.PulseTiming = volumeTest.Instrument.PulseOutputTiming;
            FirstPortBInputBoard.PulseTiming = volumeTest.Instrument.PulseOutputTiming;

            volumeTest.PulseACount = 0;
            volumeTest.PulseBCount = 0;
        }

        #endregion
    }
}
