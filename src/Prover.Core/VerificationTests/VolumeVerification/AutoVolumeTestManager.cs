using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using MccDaq;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Communication;
using Prover.Core.ExternalDevices.DInOutBoards;
using Prover.Core.Models.Instruments;
using Prover.Core.Settings;

namespace Prover.Core.VerificationTests.VolumeVerification
{
    public sealed class AutoVolumeTestManager : VolumeTestManager
    {
        private readonly IDInOutBoard _outputBoard;            
        private readonly TachometerService _tachometerCommunicator;

        public AutoVolumeTestManager(IEventAggregator eventAggregator, TachometerService tachComm)
            : base(eventAggregator)
        {
            _tachometerCommunicator = tachComm;

            FirstPortAInputBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortA, 0);
            FirstPortBInputBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortB, 1);

            _outputBoard = DInOutBoardFactory.CreateBoard(0, 0, 0);            
        }
       
        protected override async Task ExecuteSyncTest(EvcCommunicationClient commClient, VolumeTest volumeTest, CancellationToken ct)
        {
            try
            {            
                Status.OnNext("Running volume sync test...");                

                await commClient.Disconnect();                

                await Task.Run(() =>
                {
                    ResetPulseCounts(volumeTest);
                    _outputBoard.StartMotor();
                    do
                    {
                        volumeTest.PulseACount += FirstPortAInputBoard.ReadInput();
                        volumeTest.PulseBCount += FirstPortBInputBoard.ReadInput();
                    } while (volumeTest.UncPulseCount < 1 && !ct.IsCancellationRequested);
                }, ct);          
            }
            catch (OperationCanceledException ex)
            {
                Status.OnNext("Volume Sync test cancelled.");
                throw;
            }
            finally
            {
                _outputBoard.StopMotor();
            }
        }

        protected override async Task PreTest(EvcCommunicationClient commClient, VolumeTest volumeTest,
            CancellationToken ct)
        {
            await commClient.Connect(ct);

            volumeTest.Items = (ICollection<ItemValue>) await commClient.GetItemValues(commClient.ItemDetails.VolumeItems());
            await commClient.Disconnect();

            if (_tachometerCommunicator != null)
                await _tachometerCommunicator?.ResetTach();

            ResetPulseCounts(volumeTest);
        }

        protected override async Task ExecutingTest(VolumeTest volumeTest, CancellationToken ct)
        {
            TestStep.OnNext(VolumeTestSteps.ExecutingTest);
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
                    } while (volumeTest.UncPulseCount < volumeTest.DriveType.MaxUncorrectedPulses() && !ct.IsCancellationRequested);
                }, ct);     
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

        protected override async Task PostTest(EvcCommunicationClient commClient, VolumeTest volumeTest,
            CancellationToken ct)
        {
            TestStep.OnNext(VolumeTestSteps.PostTest);
            ct.ThrowIfCancellationRequested();
            await Task.Run(async () =>
            {
                try
                {
                    await commClient.Connect(ct);
                    volumeTest.AfterTestItems = await commClient.GetItemValues(commClient.ItemDetails.VolumeItems());
                }
                finally
                {
                    await commClient.Disconnect();
                }

                await GetAppliedInput(volumeTest);
            }, ct);
        }

        public override void Dispose()
        {
            base.Dispose();
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