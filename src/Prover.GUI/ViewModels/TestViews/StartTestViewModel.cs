using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Events;
using Prover.Core.Models.Instruments;
using Prover.Core.Settings;
using Prover.Core.VerificationTests;
using Prover.Core.VerificationTests.Mechanical;
using Prover.GUI.ViewModels.SettingsViews;
using Prover.GUI.ViewModels.Shell;
using Prover.GUI.ViewModels.VerificationTestViews;
using Prover.SerialProtocol;
using System.Threading.Tasks;
using Prover.GUI.ProgressDialog;

namespace Prover.GUI.ViewModels.TestViews
{
    public class StartTestViewModel : ReactiveScreen, IHandle<SettingsChangeEvent>
    {
        private IUnityContainer _container;
        private bool _miniAtChecked;

        public BaudRateEnum BaudRate { get; private set; }
        public ICommPort CommPort { get; set; }
        public string InstrumentCommPortName { get; private set; }

        public TestManager InstrumentTestManager { get; set; }
        public string TachCommPortName { get; private set; }

        public bool IsMiniMaxChecked
        {
            get
            {
                return SettingsManager.SettingsInstance.LastInstrumentTypeUsed == "MiniMax";
            }
            set
            {
                if (value) SettingsManager.SettingsInstance.LastInstrumentTypeUsed = "MiniMax";
            }
        }
        public bool IsMiniATChecked
        {
            get
            {
                return SettingsManager.SettingsInstance.LastInstrumentTypeUsed == "MiniAT";
            }
            set
            {
                if (value) SettingsManager.SettingsInstance.LastInstrumentTypeUsed = "MiniAT";   
            }
        }

        public StartTestViewModel(IUnityContainer container)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public async Task CancelCommand()
        {
            await ScreenManager.Change(_container, new MainMenuViewModel(_container));
        }

        public void Handle(SettingsChangeEvent message)
        {
            VerifySettings();
        }

        public async Task InitializeTest()
        {
            SettingsManager.Save();

            var commPort = Communications.CreateCommPortObject(InstrumentCommPortName, BaudRate);

            if (IsMiniMaxChecked)
                InstrumentTestManager = await RotaryTestManager.CreateRotaryTest(_container, InstrumentType.MiniMax, commPort, TachCommPortName);
            else if (IsMiniATChecked)
                InstrumentTestManager = await MechanicalTestManager.Create(_container, InstrumentType.MiniAt, commPort, TachCommPortName);

            await ScreenManager.Change(_container, new VerificationTestViewModel(_container, InstrumentTestManager));
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

            base.NotifyOfPropertyChange(() => InstrumentCommPortName);
            base.NotifyOfPropertyChange(() => BaudRate);
            base.NotifyOfPropertyChange(() => TachCommPortName);

            if (string.IsNullOrEmpty(InstrumentCommPortName))
            {
                ScreenManager.ShowDialog(_container, new SettingsViewModel(_container));
            }
        }

    }
}
