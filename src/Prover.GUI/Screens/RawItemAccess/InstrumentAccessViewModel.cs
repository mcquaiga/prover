using System;
using System.Dynamic;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.Core.Settings;
using Prover.GUI.Common.Events;

namespace Prover.GUI.Screens.RawItemAccess
{
    public class InstrumentAccessViewModel : ReactiveScreen, IHandle<ScreenChangeEvent>, IDisposable
    {
        public InstrumentAccessViewModel()
        {
            SetupCommPort().Wait();
        }

        public InstrumentAccessViewModel(IContainer _container)
        {
        }

        public EvcCommunicationClient InstrumentCommunicator { get; private set; }

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

        public void Dispose()
        {
            Task.Run(async () => await DisconnectFromInstrument());
        }

        public void Handle(ScreenChangeEvent message)
        {
            
        }

        public async Task ReadInstrumentValue()
        {
            //await InstrumentCommunicator.Connect();
            //var result = await InstrumentCommunicator.GetItemValue(ItemNumber);
            //ItemValue = result.RawValue;
            NotifyOfPropertyChange(() => ItemValue);
        }

        public async Task WriteInstrumentValue()
        {
            await InstrumentCommunicator.Connect();

            await InstrumentCommunicator.SetItemValue(ItemNumber, ItemValue);
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
                    var commPort = new SerialPort(SettingsManager.SettingsInstance.InstrumentCommPort,
                        SettingsManager.SettingsInstance.InstrumentBaudRate);
                    //InstrumentCommunicator = new HoneywellClient(commPort, InstrumentTypes.MiniMax);
                    //var commPort = Communications.CreateCommPortObject(SettingsManager.SettingsInstance.InstrumentCommPort, SettingsManager.SettingsInstance.InstrumentBaudRate);
                    //InstrumentCommunicator = new InstrumentCommunicator(_container.Resolve<IEventAggregator>(), commPort, InstrumentType.MiniMax);
                }
            });
        }
    }
}