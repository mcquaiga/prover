namespace Prover.Core.VerificationTests.VolumeVerification
{
    using Caliburn.Micro;
    using Prover.CommProtocol.Common;
    using Prover.Core.Communication;
    using Prover.Core.ExternalDevices.DInOutBoards;
    using Prover.Core.Models.Instruments;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="AutoVolumeTestManager" />
    /// </summary>
    public class AutoVolumeTestManager : VolumeTestManager
    {
        #region Fields

        /// <summary>
        /// Defines the _outputBoard
        /// </summary>
        private readonly IDInOutBoard _outputBoard;

        /// <summary>
        /// Defines the _tachometerCommunicator
        /// </summary>
        private readonly TachometerService _tachometerCommunicator;
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
            _tachometerCommunicator = tachComm;
            _outputBoard = DInOutBoardFactory.CreateBoard(0, 0, 0);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The Dispose
        /// </summary>
        public override void Dispose()
        {
            _tachometerCommunicator.Dispose();
        }

        /// <summary>
        /// The PostTest
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <param name="evcPostTestItemReset">The evcPostTestItemReset<see cref="IEvcItemReset"/></param>
        /// <param name="readTach">The readTach<see cref="bool"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public override async Task PostTest(EvcCommunicationClient commClient, VolumeTest volumeTest, IEvcItemReset evcPostTestItemReset, CancellationToken ct, bool readTach = true)
        {
            try
            {
                await commClient.Connect();
                
                await CheckForResidualPulses(commClient, volumeTest, ct);
                          
                 volumeTest.Items = await commClient.GetVolumeItems();
                //if (volumeTest.VerificationTest.FrequencyTest != null)
                //{
                //    volumeTest.VerificationTest.FrequencyTest.PostTestItemValues = await commClient.GetFrequencyItems();
                //}

                if (evcPostTestItemReset != null) await evcPostTestItemReset.PostReset(commClient);
            }
            finally
            {
                await commClient.Disconnect();
            }

            if (readTach) await GetAppliedInput(volumeTest);
        }

        /// <summary>
        /// The PreTest
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <param name="evcTestItemReset">The evcTestItemReset<see cref="IEvcItemReset"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public override async Task PreTest(EvcCommunicationClient commClient, VolumeTest volumeTest,
            IEvcItemReset evcTestItemReset)
        {
            await commClient.Connect();

            if (evcTestItemReset != null)
                await evcTestItemReset.PreReset(commClient);

            volumeTest.Items = await commClient.GetVolumeItems();

            if (volumeTest.VerificationTest.FrequencyTest != null)
            {
                volumeTest.VerificationTest.FrequencyTest.PreTestItemValues = await commClient.GetFrequencyItems();
            }

            await commClient.Disconnect();

            if (_tachometerCommunicator != null)
                await _tachometerCommunicator?.ResetTach();

            ResetPulseCounts(volumeTest);
        }

        /// <summary>
        /// The ExecuteSyncTest
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        protected override async Task ExecuteSyncTest(EvcCommunicationClient commClient, VolumeTest volumeTest, CancellationToken ct)
        {
            try
            {
                await commClient.Disconnect();

                Log.Info("Running volume sync test...");

                await Task.Run(() =>
                {
                    ResetPulseCounts(volumeTest);
                    _outputBoard?.StartMotor();
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
                _outputBoard.StopMotor();
            }
        }

        /// <summary>
        /// The ExecutingTest
        /// </summary>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <param name="ct">The ct<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        protected override async Task ExecutingTest(VolumeTest volumeTest, CancellationToken ct)
        {
            try
            {
                await Task.Run(() =>
                {
                    _outputBoard?.StartMotor();

                    _pulseInputsCancellationTokenSource = new CancellationTokenSource();
                    Task.Run(() => ListenForPulseInputs(volumeTest, _pulseInputsCancellationTokenSource.Token));
                    do
                    {
                    } while ((volumeTest.UncPulseCount < volumeTest.DriveType.MaxUncorrectedPulses()) && !ct.IsCancellationRequested);

                }, ct);
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
                _outputBoard?.StopMotor();
            }
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

        /// <summary>
        /// The CheckForResidualPulses
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private async Task CheckForResidualPulses(EvcCommunicationClient commClient, VolumeTest volumeTest, CancellationToken ct)
        {
            int pulsesWaiting;

            do
            {
                pulsesWaiting = 0;

                if (!commClient.IsConnected)
                    await commClient.Connect();

                foreach (var i in await commClient.GetPulseOutputItems())
                {
                    pulsesWaiting += (int)i.NumericValue;
                }
                    
                if (pulsesWaiting > 0)
                {
                    await commClient.Disconnect();
                    await Task.Delay(new TimeSpan(0, 0, 60), ct);                    
                }                

            } while (pulsesWaiting > 0 && !ct.IsCancellationRequested);

            _pulseInputsCancellationTokenSource.Cancel();
        }

        /// <summary>
        /// The GetAppliedInput
        /// </summary>
        /// <param name="volumeTest">The volumeTest<see cref="VolumeTest"/></param>
        /// <returns>The <see cref="Task"/></returns>
        private async Task GetAppliedInput(VolumeTest volumeTest)
        {
            if (_tachometerCommunicator == null) return;

            int? result = null;
            var tries = 0;
            do
            {
                try
                {
                    tries++;
                    Log.Debug($"Reading tachometer .... Attempt {tries} of 10");
                    result = await _tachometerCommunicator.ReadTach();
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
            do
            {
                //TODO: Raise events so the UI can respond
                volumeTest.PulseACount += FirstPortAInputBoard.ReadInput();
                volumeTest.PulseBCount += FirstPortBInputBoard.ReadInput();
            } while (!ct.IsCancellationRequested);

            return ct;
        }

        #endregion
    }
}
