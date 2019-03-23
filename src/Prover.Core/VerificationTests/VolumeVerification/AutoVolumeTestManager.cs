namespace Prover.Core.VerificationTests.VolumeVerification
{
    using Caliburn.Micro;
    using MccDaq;
    using Prover.CommProtocol.Common;
    using Prover.Core.ExternalDevices;
    using Prover.Core.ExternalDevices.DInOutBoards;
    using Prover.Core.Models.Instruments;
    using Prover.Core.Settings;
    using System;
    using System.Reactive.Concurrency;
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
        /// Defines the OutputBoard
        /// </summary>
        protected readonly IDInOutBoard OutputBoard;

        /// <summary>
        /// Defines the TachometerCommunicator
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
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
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

        #endregion

        #region Methods

        /// <summary>
        /// The PostTest
        /// </summary>
        /// <param name="testActionsManager">The testActionsManager<see cref="ITestActionsManager"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public override async Task CompleteTest(ITestActionsManager testActionsManager, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            Status.OnNext("Completing volume test...");
           
            try
            {
                await CommClient.Connect(ct);

                await CheckForResidualPulses(CommClient, ct);

                VolumeTest.AfterTestItems = await CommClient.GetVolumeItems();
                if (VolumeTest.VerificationTest.FrequencyTest != null)
                {
                    VolumeTest.VerificationTest.FrequencyTest.PostTestItemValues = await CommClient.GetFrequencyItems();
                }

                await testActionsManager?.RunVolumeTestCompleteActions(CommClient, VolumeTest.Instrument);
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
            base.Dispose();
            TachometerCommunicator?.Dispose();
        }

        /// <summary>
        /// The ExecuteSyncTest
        /// </summary>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public override async Task ExecuteSyncTest(CancellationToken ct)
        {
            try
            {
                Status.OnNext("Running volume sync test...");

                await CommClient.Disconnect();

                await Task.Run(() =>
                {    
                    ResetPulseCounts(VolumeTest);
                    OutputBoard.StartMotor();
                    do
                    {
                        VolumeTest.PulseACount += FirstPortAInputBoard.ReadInput();
                        VolumeTest.PulseBCount += FirstPortBInputBoard.ReadInput();
                    } while (VolumeTest.UncPulseCount < 1 && !ct.IsCancellationRequested);
                }, ct);
            }
            catch (OperationCanceledException)
            {
                Status.OnNext("Volume Sync test cancelled.");
                throw;
            }
            finally
            {
                OutputBoard.StopMotor();
            }
        }

        /// <summary>
        /// The PreTest
        /// </summary>
        /// <param name="testActionsManager">The testActionsManager<see cref="ITestActionsManager"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public override async Task PreTest(EvcCommunicationClient commClient, VolumeTest volumeTest, ITestActionsManager testActionsManager, CancellationToken ct)
        {
            CommClient = commClient;
            VolumeTest = volumeTest;

            CommClient.Status.Subscribe(Status);

            await CommClient.Connect(ct);

            await testActionsManager.RunVolumeTestInitActions(CommClient, VolumeTest.Instrument);

            VolumeTest.Items = await CommClient.GetVolumeItems();

            if (VolumeTest.VerificationTest.FrequencyTest != null)
            {
                VolumeTest.VerificationTest.FrequencyTest.PreTestItemValues = await CommClient.GetFrequencyItems();
            }

            await CommClient.Disconnect();

            if (TachometerCommunicator != null)
            {
                Status.OnNext("Resetting Tachometer...");
                await TachometerCommunicator?.ResetTach();
            }

            ResetPulseCounts(VolumeTest);
        }

        /// <summary>
        /// The ExecutingTest
        /// </summary>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public override async Task RunTest(CancellationToken ct)
        {
            _pulseInputsCancellationTokenSource = new CancellationTokenSource();
            var listen = ListenForPulseInputs(VolumeTest, _pulseInputsCancellationTokenSource.Token);

            try
            {
                ct.ThrowIfCancellationRequested();
    
                using (Observable
                    .Interval(TimeSpan.FromMilliseconds(500))
                    .Subscribe(l => Status.OnNext($"Waiting for pulse inputs... {Environment.NewLine}" +
                                                    $"   UncVol => {VolumeTest.UncPulseCount} / {VolumeTest.DriveType.MaxUncorrectedPulses()} {Environment.NewLine}" +
                                                    $"   CorVol => {VolumeTest.CorPulseCount}")))
                {
                    OutputBoard?.StartMotor();                    
                    await WaitForTestComplete(VolumeTest, ct);
                }

                ct.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                Log.Info("Cancelling volume test.");
                throw;
            }
            finally
            {
                OutputBoard?.StopMotor();                              
            }
        }

        /// <summary>
        /// The WaitForTestComplete
        /// </summary>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        protected abstract Task WaitForTestComplete(VolumeTest volumeTest, CancellationToken ct);

        /// <summary>
        /// The CheckForResidualPulses
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private async Task CheckForResidualPulses(EvcCommunicationClient commClient, CancellationToken ct)
        {
            int pulsesWaiting;
            int lastPulsesWaiting = 0; 
            bool keepWaiting = true;

            Status.OnNext("Waiting for residual pulses...");

            using (Observable                    
                   .Interval(TimeSpan.FromSeconds(10))
                   .Subscribe(async _ => {
                       pulsesWaiting = 0;

                       if (!commClient.IsConnected)
                           await commClient.Connect(ct);

                       foreach (var i in await commClient.GetPulseOutputItems())
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
        }

        /// <summary>
        /// The GetAppliedInput
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        private async Task GetAppliedInput()
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

        #endregion
    }
}
