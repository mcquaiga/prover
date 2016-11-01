using System;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.DriveTypes;
using Prover.Core.Events;
using Prover.Core.Settings;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Screens.QAProver.VerificationTestViews;
using Prover.GUI.Screens.Settings;

namespace Prover.GUI.Screens.QAProver
{
    public class NewQaTestRunViewModel : ViewModelBase, IHandle<SettingsChangeEvent>
    {
        public NewQaTestRunViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
            : base(screenManager, eventAggregator)
        {
        }

        public int BaudRate { get; private set; }
        public CommPort CommPort { get; set; }
        public string InstrumentCommPortName { get; private set; }
        public string TachCommPortName { get; private set; }
        
        public async Task CancelCommand()
        {
            await ScreenManager.GoHome();
        }

        public async Task InitializeTest()
        {
            SettingsManager.Save();

            try
            {
                var qaTestRun = ScreenManager.ResolveViewModel<QaTestRunInteractiveViewModel>();
                await qaTestRun.Initialize(Instruments.MiniAt, new MechanicalDrive());
                await ScreenManager.ChangeScreen(qaTestRun);
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
                ScreenManager.ShowWindow(new SettingsViewModel(ScreenManager, EventAggregator));
        }

        public void Handle(SettingsChangeEvent message)
        {
            VerifySettings();
        }
    }
}