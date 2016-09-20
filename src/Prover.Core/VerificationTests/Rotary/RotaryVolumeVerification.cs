using MccDaq;
using NLog;
using Prover.Core.Communication;
using Prover.Core.ExternalDevices.DInOutBoards;
using Prover.Core.Models.Instruments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Core.Models;
using Caliburn.Micro;

namespace Prover.Core.VerificationTests
{
    public sealed class RotaryVolumeVerification : BaseVolumeVerificationManager
    {
        private IDInOutBoard _outputBoard;
        private TachometerCommunicator _tachometerCommunicator;
       
        public RotaryVolumeVerification(IEventAggregator eventAggregator, VolumeTest volumeTest, InstrumentCommunicator instrumentComm, TachometerCommunicator tachComm)
            :base(eventAggregator, volumeTest, instrumentComm)
        {
            _tachometerCommunicator = tachComm;            
            _outputBoard = DInOutBoardFactory.CreateBoard(0, 0, 0);
        }

        public override async Task StartVolumeTest()
        {
            if (!_runningTest)
            {
                _log.Info("Starting volume test...");
                await RunSyncTest();

                VolumeTest.ItemValues = await _instrumentCommunicator.DownloadItemsAsync(_volumeItems);

                await _instrumentCommunicator.Disconnect();

                //Reset Tach setting
                await _tachometerCommunicator?.ResetTach();

                VolumeTest.PulseACount = 0;
                VolumeTest.PulseBCount = 0;

                _outputBoard.StartMotor();
                _runningTest = true;

                await RunningVolumeTest();
            }
        }

        public override async Task RunningVolumeTest()
        {
            await Task.Run(async () =>
            {
                do
                {
                    //TODO: Raise events so the UI can respond
                    VolumeTest.PulseACount += _firstPortAInputBoard.ReadInput();
                    VolumeTest.PulseBCount += _firstPortBInputBoard.ReadInput();
                } while (VolumeTest.UncPulseCount < VolumeTest.DriveType.MaxUnCorrected() && !_requestStopTest);

                _outputBoard?.StopMotor();
                await FinishVolumeTest();
            });
        }

        public override async Task FinishVolumeTest()
        {
            await Task.Run(async () =>
            {
                try
                {
                    System.Threading.Thread.Sleep(250);

                    VolumeTest.AfterTestItemValues = await _instrumentCommunicator.DownloadItemsAsync(_volumeItems);
                    await ZeroInstrumentVolumeItems(); 

                    try
                    {
                        VolumeTest.AppliedInput = await _tachometerCommunicator?.ReadTach();
                        _log.Info(string.Format("Uncorrected % Error: {0}", VolumeTest.UnCorrectedPercentError));
                        _log.Info(string.Format("Tachometer reading: {0}", VolumeTest.AppliedInput));
                    }
                    catch (Exception ex)
                    {
                        _log.Error(string.Format("An error occured communication with the tachometer: {0}", ex));
                    }

                    _log.Info("Volume test finished!");
                }
                finally
                {
                    _runningTest = false;
                    await _instrumentCommunicator.Disconnect();
                }
            });
        }

        private async Task RunSyncTest()
        {
            await Task.Run(async () =>
            {
                if (!_runningTest)
                {
                    _log.Info("Syncing volume...");

                    await _instrumentCommunicator.Disconnect();
                    _outputBoard.StartMotor();

                    VolumeTest.PulseACount = 0;
                    VolumeTest.PulseBCount = 0;

                    do
                    {
                        VolumeTest.PulseACount += _firstPortAInputBoard.ReadInput();
                        VolumeTest.PulseBCount += _firstPortBInputBoard.ReadInput();
                    } while (VolumeTest.UncPulseCount < 1);

                    _outputBoard.StopMotor();
                    System.Threading.Thread.Sleep(500);
                }
            });
        }

        protected override async Task ZeroInstrumentVolumeItems()
        {
            await _instrumentCommunicator.WriteItem(264, "20140867", false);
            await _instrumentCommunicator.WriteItem(434, "0", false);
            await _instrumentCommunicator.WriteItem(113, "0", false);
            await _instrumentCommunicator.WriteItem(892, "0", false);
            await base.ZeroInstrumentVolumeItems();
        }  
    }
}
