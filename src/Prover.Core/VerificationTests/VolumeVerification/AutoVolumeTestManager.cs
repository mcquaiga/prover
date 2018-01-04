using System;
using System.Collections.Generic;
using System.Reactive.Linq;
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

        public AutoVolumeTestManager(IEventAggregator eventAggregator, EvcCommunicationClient commClient, VolumeTest volumeTest, TachometerService tachComm, ISettingsService settingsService) 
            : base(eventAggregator, commClient, volumeTest, settingsService)
        {
            _tachometerCommunicator = tachComm;

            FirstPortAInputBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortA, 0);
            FirstPortBInputBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortB, 1);

            _outputBoard = DInOutBoardFactory.CreateBoard(0, 0, 0);
        }

        public override async Task ExecuteSyncTest(CancellationToken ct)
        {
            try
            {            
                Status.OnNext("Running volume sync test...");                

                await CommClient.Disconnect();                

                await Task.Run(() =>
                {
                    ResetPulseCounts(VolumeTest);
                    _outputBoard.StartMotor();
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
                _outputBoard.StopMotor();
            }
        }

        public override async Task PreTest(CancellationToken ct)
        {
            CommClient.StatusObservable.Subscribe(Status);
            await CommClient.Connect(ct);
            VolumeTest.Items = await CommClient.GetVolumeItems();

            if (VolumeTest.VerificationTest.FrequencyTest != null)
            {
                VolumeTest.VerificationTest.FrequencyTest.PreTestItemValues = await CommClient.GetFrequencyItems();
            }

            await CommClient.Disconnect();

            if (_tachometerCommunicator != null)
            {
                Status.OnNext("Resetting Tachometer...");
                await _tachometerCommunicator?.ResetTach();
            }
            ResetPulseCounts(VolumeTest);
        }

        public override async Task ExecutingTest(CancellationToken ct)
        {
            try
            {
                ct.ThrowIfCancellationRequested();                
                await Task.Run(() =>
                {
                    using (Observable
                        .Interval(TimeSpan.FromMilliseconds(100))
                        .Subscribe(l => Status.OnNext($"Waiting for pulse inputs... {Environment.NewLine}" +
                                                      $"   UncVol => {VolumeTest.UncPulseCount} / {VolumeTest.DriveType.MaxUncorrectedPulses()} {Environment.NewLine}" +
                                                      $"   CorVol => {VolumeTest.CorPulseCount}")))
                    {
                        _outputBoard?.StartMotor();
                        do
                        {
                            //TODO: Raise events so the UI can respond
                            VolumeTest.PulseACount += FirstPortAInputBoard.ReadInput();
                            VolumeTest.PulseBCount += FirstPortBInputBoard.ReadInput();
                        } while (VolumeTest.UncPulseCount < VolumeTest.DriveType.MaxUncorrectedPulses() &&
                                    !ct.IsCancellationRequested);
                    }
                }, ct);

                ct.ThrowIfCancellationRequested();                
            }
            catch (OperationCanceledException)
            {
                Log.Info("Cancelling volume test.");
                throw;
            }
            finally
            {
                _outputBoard?.StopMotor();
            }
        }

        public override async Task PostTest(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            Status.OnNext("Completing volume test...");
            await Task.Run(async () =>
            {
                try
                {
                    await CommClient.Connect(ct);
                    VolumeTest.AfterTestItems = await CommClient.GetVolumeItems();
                    if (VolumeTest.VerificationTest.FrequencyTest != null)
                    {
                        VolumeTest.VerificationTest.FrequencyTest.PostTestItemValues = await CommClient.GetFrequencyItems();
                    }
                }
                finally
                {
                    await CommClient.Disconnect();
                }

                await GetAppliedInput();
            }, ct);
        }

        public override void Dispose()
        {
            base.Dispose();
            _tachometerCommunicator?.Dispose();
        }

        private async Task GetAppliedInput()
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
                    result = await _tachometerCommunicator?.ReadTach();
                }
                catch (Exception ex)
                {
                    Log.Error($"An error occured communication with the tachometer: {ex}");
                }
            } while (!result.HasValue && tries < 10);

            Log.Debug($"Applied Input: {result.Value}");

            VolumeTest.AppliedInput = result.Value;
        } 
        

    }
}