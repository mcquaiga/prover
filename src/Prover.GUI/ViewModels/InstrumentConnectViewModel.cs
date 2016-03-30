using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Events;
using Prover.Core.Models.Instruments;
using Prover.Core.Settings;
using Prover.Core.VerificationTests;
using Prover.GUI.Events;
using Prover.GUI.Views;
using Prover.SerialProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.GUI.ViewModels
{
    public class InstrumentConnectViewModel : ReactiveScreen, IHandle<SettingsChangeEvent>
    {
        private IUnityContainer _container;

        public InstrumentConnectViewModel(IUnityContainer container)
        {
            _container = container;
        }

        public TestManager InstrumentTestManager { get; set; }
        public ICommPort CommPort { get; set; }
        public string InstrumentCommPortName { get; private set; }
        public string TachCommPortName { get; private set; }
        public BaudRateEnum BaudRate { get; private set; }

        protected override void OnViewReady(object view)
        {
            base.OnViewReady(view);
            VerifySettings();
        }

        private void VerifySettings()
        {
            InstrumentCommPortName = SettingsManager.SettingsInstance.InstrumentCommPort;
            BaudRate = SettingsManager.SettingsInstance.InstrumentBaudRate;
            TachCommPortName = SettingsManager.SettingsInstance.TachCommPort;

            base.NotifyOfPropertyChange(() => InstrumentCommPortName);
            base.NotifyOfPropertyChange(() => BaudRate);
            base.NotifyOfPropertyChange(() => TachCommPortName);

            if (string.IsNullOrEmpty(InstrumentCommPortName))
            {
                ScreenManager.ShowDialog(_container, new SettingsViewModel(_container));
            }
        }

        public async Task CancelCommand()
        {
            await ScreenManager.Change(_container, new MainMenuViewModel(_container));
        }


        public async Task InitializeTest()
        {
            var commPort = Communications.CreateCommPortObject(InstrumentCommPortName, BaudRate);
            InstrumentTestManager = await TestManager.Create(_container, InstrumentType.MiniMax, commPort, TachCommPortName);

            await ScreenManager.Change(_container, new NewTestViewModel(_container, InstrumentTestManager));        
        }

        public void Handle(SettingsChangeEvent message)
        {
            VerifySettings();
        }

    }
}
