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

        public RotaryVolumeVerification(Instrument instrument, InstrumentCommunicator instrumentComm, TachometerCommunicator tachComm)
            :base(instrument, instrumentComm)
        {
            _tachometerCommunicator = tachComm;

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
                    await _instrumentCommunicator.DownloadItemsAsync(_instrument.Volume.Items);
                    _isFirstVolumeTest = false;
                }                   

                await _instrumentCommunicator.Disconnect();

                //Reset Tach setting
                await _tachometerCommunicator?.ResetTach();

                _instrument.Volume.PulseACount = 0;
                _instrument.Volume.PulseBCount = 0;

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
                    _instrument.Volume.PulseACount += _firstPortAInputBoard.ReadInput();
                    _instrument.Volume.PulseBCount += _firstPortBInputBoard.ReadInput();
                } while (_instrument.Volume.UncPulseCount < _instrument.Volume.MaxUnCorrected() || _stopTest);

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

                    await _instrumentCommunicator.DownloadItemsAsync(_instrument.Volume.AfterTestItems);

                    try
                    {
                        _instrument.Volume.AppliedInput = await _tachometerCommunicator?.ReadTach();
                        _log.Info(string.Format("Tachometer reading: {0}", _instrument.Volume?.AppliedInput));
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
