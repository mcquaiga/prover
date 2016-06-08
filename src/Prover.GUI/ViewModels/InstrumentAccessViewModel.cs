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
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.MiHoneywell;

namespace Prover.GUI.ViewModels
{
    public class InstrumentAccessViewModel : ReactiveScreen, IHandle<ScreenChangeEvent>, IDisposable
    {
        public EvcCommunicationClient InstrumentCommunicator { get; private set; }

        public InstrumentAccessViewModel()
        {
            SetupCommPort().Wait();
        }

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
        }

        public override void CanClose(Action<bool> callback)
        {
            DisconnectFromInstrument().Wait();
            base.CanClose(callback);
        }

        public async Task ReadInstrumentValue()
        {
            await InstrumentCommunicator.Connect();
            var result = await InstrumentCommunicator.GetItemValue(ItemNumber);
            ItemValue = result.RawValue;
            base.NotifyOfPropertyChange(() => ItemValue);
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
                    var commPort = new SerialPortV2(SettingsManager.SettingsInstance.InstrumentCommPort, SettingsManager.SettingsInstance.InstrumentBaudRate);
                    InstrumentCommunicator = new HoneywellClient(commPort, InstrumentType.MiniMax);
                    //var commPort = Communications.CreateCommPortObject(SettingsManager.SettingsInstance.InstrumentCommPort, SettingsManager.SettingsInstance.InstrumentBaudRate);
                    //InstrumentCommunicator = new InstrumentCommunicator(_container.Resolve<IEventAggregator>(), commPort, InstrumentType.MiniMax);
                }
            });
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