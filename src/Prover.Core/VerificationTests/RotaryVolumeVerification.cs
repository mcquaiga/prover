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
    sealed class RotaryVolumeVerification : VerificationBase
    {
        private Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private IEventAggregator _eventAggreator;
        private bool _isFirstVolumeTest = true;
        private bool _runningTest = false;

        private IDInOutBoard _outputBoard;
        private IDInOutBoard _firstPortAInputBoard;
        private IDInOutBoard _firstPortBInputBoard;
        private TachometerCommunicator _tachometerCommunicator;
        private bool _stopTest;
        private VerificationTest _verificationTest;
        private List<ItemDetail> _volumeItems;

        public RotaryVolumeVerification(IEventAggregator eventAggregator, VerificationTest verificationTest, InstrumentCommunicator instrumentComm, TachometerCommunicator tachComm)
            :base(verificationTest.Instrument, instrumentComm)
        {
            _eventAggreator = eventAggregator;
            _tachometerCommunicator = tachComm;
            _verificationTest = verificationTest;
            _verificationTest.VolumeTest = new VolumeTest(_verificationTest);
            _outputBoard = DInOutBoardFactory.CreateBoard(0, 0, 0);
            _firstPortAInputBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortA, 0);
            _firstPortBInputBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortB, 1);

            _volumeItems = _verificationTest.Instrument.Items.Items.Where(i => i.IsVolumeTest == true).ToList();
        }

        public async Task BeginVerificationTest()
        {
            if (!_runningTest)
            {
                _log.Info("Starting volume test...");
                await RunSyncTest();

                _verificationTest.VolumeTest.ItemValues = await _instrumentCommunicator.DownloadItemsAsync(_volumeItems);

                await _instrumentCommunicator.Disconnect();

                //Reset Tach setting
                await _tachometerCommunicator?.ResetTach();

                _verificationTest.VolumeTest.PulseACount = 0;
                _verificationTest.VolumeTest.PulseBCount = 0;

                _outputBoard.StartMotor();
                _runningTest = true;

                await RunningTest();
            }
        }

        private async Task RunSyncTest()
        {
            if (!_runningTest)
            {
                _log.Info("Syncing volume...");

                await _instrumentCommunicator.Disconnect();
                _outputBoard.StartMotor();

                _verificationTest.VolumeTest.PulseACount = 0;
                _verificationTest.VolumeTest.PulseBCount = 0;

                do
                {
                    _verificationTest.VolumeTest.PulseACount += _firstPortAInputBoard.ReadInput();
                    _verificationTest.VolumeTest.PulseBCount += _firstPortBInputBoard.ReadInput();
                } while (_verificationTest.VolumeTest.UncPulseCount < 1);

                _outputBoard.StopMotor();
                System.Threading.Thread.Sleep(500);
            }
        }

        private async Task ClearInstrumentValues()
        {
            await _instrumentCommunicator.WriteItem(264, "20140867", false);
            await _instrumentCommunicator.WriteItem(434, "0", false);
            await _instrumentCommunicator.WriteItem(892, "0", false);
            await _instrumentCommunicator.WriteItem(0, "0", false);
            await _instrumentCommunicator.WriteItem(2, "0", false);
            await _instrumentCommunicator.WriteItem(113, "0", false);
        }

        public void StopRunningTest()
        {
            _stopTest = true;
        }

        private async Task RunningTest()
        {
            await Task.Run(async () =>
            {
                do
                {
                    //TODO: Raise events so the UI can respond
                    _verificationTest.VolumeTest.PulseACount += _firstPortAInputBoard.ReadInput();
                    _verificationTest.VolumeTest.PulseBCount += _firstPortBInputBoard.ReadInput();
                } while (_verificationTest.VolumeTest.UncPulseCount < _verificationTest.VolumeTest.DriveType.MaxUnCorrected() && !_stopTest);

                _outputBoard?.StopMotor();
                await FinishVerificationTest();
            });
        }
        
        public async Task FinishVerificationTest()
        {
            await Task.Run(async () =>
            {
                try
                {
                    System.Threading.Thread.Sleep(250);

                    _verificationTest.VolumeTest.AfterTestItemValues = await _instrumentCommunicator.DownloadItemsAsync(_volumeItems);

                    await ClearInstrumentValues();

                    try
                    {
                        _verificationTest.VolumeTest.AppliedInput = await _tachometerCommunicator?.ReadTach();
                        _log.Info(string.Format("Tachometer reading: {0}", _verificationTest.VolumeTest.AppliedInput));
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
                }
            });
        }        
    }
}
