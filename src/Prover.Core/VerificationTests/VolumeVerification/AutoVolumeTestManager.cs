using System;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Communication;
using Prover.Core.ExternalDevices.DInOutBoards;
using Prover.Core.Models.Instruments;

namespace Prover.Core.VerificationTests.VolumeVerification
{
    public class AutoVolumeTestManager : VolumeTestManager
    {
        private readonly IDInOutBoard _outputBoard;
        private readonly TachometerService _tachometerCommunicator;

        public AutoVolumeTestManager(IEventAggregator eventAggregator, TachometerService tachComm)
            : base(eventAggregator)
        {
            _tachometerCommunicator = tachComm;
            _outputBoard = DInOutBoardFactory.CreateBoard(0, 0, 0);
        }

        protected override async Task ExecuteSyncTest(EvcCommunicationClient commClient, VolumeTest volumeTest, CancellationToken ct)
        {
            try
            {
                await commClient.Disconnect();

                Log.Info("Running volume sync test...");
                
                FirstPortAInputBoard.PulseTiming = volumeTest.Instrument.PulseOutputTiming;
                FirstPortBInputBoard.PulseTiming = volumeTest.Instrument.PulseOutputTiming;

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

        private static void ResetPulseCounts(VolumeTest volumeTest)
        {
            volumeTest.PulseACount = 0;
            volumeTest.PulseBCount = 0;
        }

        protected override async Task ExecutingTest(VolumeTest volumeTest, CancellationToken ct)
        {
            try
            {
                await Task.Run(() =>
                {
                    _outputBoard?.StartMotor();
                    do
                    {
                        //TODO: Raise events so the UI can respond
                        volumeTest.PulseACount += FirstPortAInputBoard.ReadInput();
                        volumeTest.PulseBCount += FirstPortBInputBoard.ReadInput();
                    } while ((volumeTest.UncPulseCount < volumeTest.DriveType.MaxUncorrectedPulses()) && !ct.IsCancellationRequested);
                }, ct);
                ct.ThrowIfCancellationRequested();
            }         
            catch (OperationCanceledException ex)
            {
                Log.Info("Cancelling volume test.");
                throw;
            }
            finally
            {
                _outputBoard?.StopMotor();
            }
        }
        
        public override async Task PostTest(EvcCommunicationClient commClient, VolumeTest volumeTest, IEvcItemReset evcPostTestItemReset, bool readTach = true)
        {            
                try
                {
                    await commClient.Connect();
                    volumeTest.AfterTestItems = await commClient.GetVolumeItems();

                    if (volumeTest.VerificationTest.FrequencyTest != null)
                    {
                        volumeTest.VerificationTest.FrequencyTest.PostTestItemValues = await commClient.GetFrequencyItems();
                    }
                    
                    if (evcPostTestItemReset != null) await evcPostTestItemReset.PostReset(commClient);
                }
                finally
                {
                    await commClient.Disconnect();
                }

                if (readTach) await GetAppliedInput(volumeTest);          
        }

        public override void Dispose()
        {
            _tachometerCommunicator.Dispose();
        }

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
    }
}