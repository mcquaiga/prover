using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Events;
using Prover.Core.Models.Instruments;
using Prover.Core.Settings;
using Prover.GUI.Events;
using Prover.SerialProtocol;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MccDaq;
using Prover.Core.ExternalDevices.DInOutBoards;

namespace Prover.GUI.ViewModels
{
    public class InstrumentAccessViewModel : ReactiveScreen, IHandle<ScreenChangeEvent>, IDisposable
    {
        readonly IUnityContainer _container;
        private bool _isMotorRunning = false;
        public InstrumentCommunicator InstrumentCommunicator { get; private set; }

        public IDInOutBoard OutputBoard { get; private set; }
        public IDInOutBoard InputABoard { get; private set; }
        public IDInOutBoard InputBBoard { get; private set; }

        public long PulserACount { get; private set; }
        public long PulserBCount { get; private set; }

        public static dynamic WindowInstrumentAccess
        {
            get
            {
                dynamic InstrumentAccess = new ExpandoObject();
                InstrumentAccess.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                InstrumentAccess.ResizeMode = ResizeMode.NoResize;
                InstrumentAccess.MinWidth = 450;
                InstrumentAccess.Title = @"InstrumentAccess";
                return InstrumentAccess;
            }
        }
        
        public int ItemNumber { get; set; }
        public string ItemValue { get; set; }

        public InstrumentAccessViewModel(IUnityContainer _container)
        {
            this._container = _container;
            SetupDAQBoard();
        }

        public override void CanClose(Action<bool> callback)
        {
            DisconnectFromInstrument().Wait();
            base.CanClose(callback);
        }

        public async Task ReadInstrumentValue()
        {
            await SetupCommPort();
            ItemValue = await InstrumentCommunicator.DownloadItem(ItemNumber, false);
            base.NotifyOfPropertyChange(() => ItemValue);
        }

        public async Task WriteInstrumentValue()
        {
            await SetupCommPort();
            await InstrumentCommunicator.WriteItem(ItemNumber, ItemValue, false);

        }

        public async Task DisconnectFromInstrument()
        {
            if (InstrumentCommunicator != null)
                await InstrumentCommunicator.Disconnect();
        }

        private async Task SetupCommPort()
        {
            await Task.Run(() =>
            {
                if (InstrumentCommunicator == null)
                {
                    var commPort = Communications.CreateCommPortObject(SettingsManager.SettingsInstance.InstrumentCommPort, SettingsManager.SettingsInstance.InstrumentBaudRate);
                    InstrumentCommunicator = new InstrumentCommunicator(_container.Resolve<IEventAggregator>(), commPort, InstrumentType.MiniMax);
                }
            });      
                  
        }

        public async Task ListenForPulses()
        {
            await Task.Run(async () =>
            {
                do
                {
                    //TODO: Raise events so the UI can respond
                    PulserACount += await InputABoard.ReadInput();
                    PulserBCount += await InputBBoard.ReadInput();
                    NotifyOfPropertyChange(() => PulserACount);
                    NotifyOfPropertyChange(() => PulserBCount);
                    NotifyOfPropertyChange(() => InputABoard);
                    NotifyOfPropertyChange(() => InputBBoard);
                } while (true);
            });
        }

        public async Task StartStopMotor()
        {
            await Task.Run(() =>
            {
                if (!_isMotorRunning)
                {
                    OutputBoard.StartMotor();
                    _isMotorRunning = true;
                }
                else
                {
                    OutputBoard.StopMotor();
                    _isMotorRunning = false;
                }
            });
        }

        private void SetupDAQBoard()
        {
            OutputBoard = DInOutBoardFactory.CreateBoard(0, 0, 0);
            InputABoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortA, 0);
            InputBBoard = DInOutBoardFactory.CreateBoard(0, DigitalPortType.FirstPortB, 1);
        }

        public void Handle(ScreenChangeEvent message)
        {
            DisconnectFromInstrument();
        }

        public void Dispose()
        {
            DisconnectFromInstrument();
        }
    }
}