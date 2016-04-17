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

namespace Prover.Core.VerificationTests
{
    sealed class RotaryVolumeVerification : VerificationBase
    {
        private Logger _log = LogManager.GetCurrentClassLogger();
        private bool _isFirstVolumeTest = true;
        private bool _runningTest = false;

        private IDInOutBoard _outputBoard;
        private IDInOutBoard _firstPortAInputBoard;
        private IDInOutBoard _firstPortBInputBoard;
        private TachometerCommunicator _tachometerCommunicator;
        private bool _stopTest;
        private VerificationTest _verificationTest;

        public RotaryVolumeVerification(VerificationTest verificationTest, InstrumentCommunicator instrumentComm, TachometerCommunicator tachComm)
            :base(verificationTest.Instrument, instrumentComm)
        {
            _tachometerCommunicator = tachComm;
            _verificationTest = verificationTest;
            _verificationTest.VolumeTest = new VolumeTest(_verificationTest);
            _outputBoard = DInOutBoardFactory.CreateBoard(0, 0, 0);
            _firstPortAInputBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortA, 0);
            _firstPortBInputBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortB, 1);
        }

        public async Task BeginVerificationTest()
        {
            if (!_runningTest)
            {
                _log.Info("Starting volume test...");

                if (!_isFirstVolumeTest)
                {
                    await _instrumentCommunicator.DownloadItemsAsync(_verificationTest.VolumeTest.Items);
                    _isFirstVolumeTest = false;
                }                   

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
                } while (_verificationTest.VolumeTest.UncPulseCount < _verificationTest.VolumeTest.DriveType.MaxUnCorrected() || _stopTest);

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

                    await _instrumentCommunicator.DownloadItemsAsync(_verificationTest.VolumeTest.AfterTestItems);

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
