using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Events;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Settings;
using Prover.Core.VerificationTests;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Common.Screens.MainMenu;
using Prover.GUI.Screens.QAProver.VerificationTestViews;
using Prover.GUI.Screens.Settings;
using Prover.GUI.Screens.Shell;

namespace Prover.GUI.Screens.QAProver
{
    public class StartTestViewModel : ViewModelBase, IHandle<SettingsChangeEvent>
    {
        
        public StartTestViewModel(ScreenManager screenManager, IEventAggregator eventAggregator) : base(screenManager, eventAggregator)
        {
        }

        public int BaudRate { get; private set; }
        public CommPort CommPort { get; set; }
        public string InstrumentCommPortName { get; private set; }

        public QaRunTestManager InstrumentQaRunTestManager { get; set; }
        public string TachCommPortName { get; private set; }

        public bool IsMiniMaxChecked
        {
            get { return SettingsManager.SettingsInstance.LastInstrumentTypeUsed == "MiniMax"; }
            set
            {
                if (value) SettingsManager.SettingsInstance.LastInstrumentTypeUsed = "MiniMax";
            }
        }

        public bool IsMiniATChecked
        {
            get { return SettingsManager.SettingsInstance.LastInstrumentTypeUsed == "MiniAT"; }
            set
            {
                if (value) SettingsManager.SettingsInstance.LastInstrumentTypeUsed = "MiniAT";
            }
        }

        public bool IsEC350Checked
        {
            get { return SettingsManager.SettingsInstance.LastInstrumentTypeUsed == "EC350"; }
            set
            {
                if (value) SettingsManager.SettingsInstance.LastInstrumentTypeUsed = "EC350";
            }
        }

        public void Handle(SettingsChangeEvent message)
        {
            VerifySettings();
        }

       
        public async Task CancelCommand()
        {
            await ScreenManager.GoHome();
        }

        public async Task InitializeTest()
        {
            SettingsManager.Save();

            try
            {
                //ScreenManager.ChangeScreen(new QATestRunInteractiveViewModel(InstrumentQaRunTestManager));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }

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

            NotifyOfPropertyChange(() => InstrumentCommPortName);
            NotifyOfPropertyChange(() => BaudRate);
            NotifyOfPropertyChange(() => TachCommPortName);

            if (string.IsNullOrEmpty(InstrumentCommPortName))
            {
                ScreenManager.ShowWindow(new SettingsViewModel(ScreenManager, EventAggregator));
            }
        }
    }
}