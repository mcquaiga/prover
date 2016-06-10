using System;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Events;
using Prover.Core.Settings;
using Prover.Core.VerificationTests;
using Prover.Core.VerificationTests.Mechanical;
using Prover.Core.VerificationTests.Rotary;
using Prover.GUI.ViewModels.SettingsViews;
using Prover.GUI.ViewModels.Shell;
using Prover.GUI.ViewModels.VerificationTestViews;

namespace Prover.GUI.ViewModels.TestViews
{
    public class StartTestViewModel : ReactiveScreen, IHandle<SettingsChangeEvent>
    {
        private readonly IUnityContainer _container;
        private bool _miniAtChecked;

        public StartTestViewModel(IUnityContainer container)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public int BaudRate { get; private set; }
        public CommPort CommPort { get; set; }
        public string InstrumentCommPortName { get; private set; }

        public TestManager InstrumentTestManager { get; set; }
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

        public void Handle(SettingsChangeEvent message)
        {
            VerifySettings();
        }

        public async Task CancelCommand()
        {
            await ScreenManager.Change(_container, new MainMenuViewModel(_container));
        }

        public async Task InitializeTest()
        {
            SettingsManager.Save();

            try
            {
                var commPort = new SerialPortV2(InstrumentCommPortName, BaudRate);

                if (IsMiniMaxChecked)
                {
                    InstrumentTestManager =
                        await
                            RotaryTestManager.CreateRotaryTest(_container,
                                new HoneywellClient(commPort, InstrumentType.MiniMax), TachCommPortName);
                }
                else if (IsMiniATChecked)
                {
                    InstrumentTestManager =
                        await
                            MechanicalTestManager.Create(_container,
                                new HoneywellClient(commPort, InstrumentType.MiniAT));
                }
                await ScreenManager.Change(_container, new VerificationTestViewModel(_container, InstrumentTestManager));
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
                ScreenManager.ShowDialog(_container, new SettingsViewModel(_container));
            }
        }
    }
}