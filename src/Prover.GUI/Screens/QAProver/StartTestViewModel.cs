using System;
using System.Threading.Tasks;
using System.Windows;
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
using Prover.GUI.Screens.QAProver.VerificationTestViews;
using Prover.GUI.Screens.Settings;
using Prover.GUI.Screens.Shell;

namespace Prover.GUI.Screens.QAProver
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
            await ScreenManager.Change(_container, new MainMenuViewModel(_container));
        }

        public async Task InitializeTest()
        {
            SettingsManager.Save();

            try
            {
                if (IsEC350Checked)
                {
                    var irdaPort = new IrDAPort();

                    //InstrumentQaRunTestManager =
                    //    await QaRunTestManager.Create(_container, new HoneywellClient(irdaPort, InstrumentTypes.EC350));
                }
                else
                {
                    //var commPort = new SerialPort(InstrumentCommPortName, BaudRate);

                    //if (IsMiniMaxChecked)
                    //{
                    //    InstrumentQaRunTestManager = new QaRunTestManager(
                    //        _container,
                    //        new HoneywellClient(commPort, InstrumentTypes.MiniMax),
                    //        TachCommPortName,
                    //        null);
                    //    //await
                    //    //    RotaryQaRunTestManager.CreateRotaryTest(_container,
                    //    //        new HoneywellClient(commPort, InstrumentType.MiniMax), TachCommPortName,
                    //    //        null);

                    //    await InstrumentQaRunTestManager.RunVerifier();
                    //}
                    //else if (IsMiniATChecked)
                    //{
                    //    InstrumentQaRunTestManager =
                    //        await
                    //            MechanicalQaRunTestManager.Create(_container,
                    //                new HoneywellClient(commPort, InstrumentTypes.MiniAT));
                    //}
                }                
                
                await ScreenManager.Change(_container, new VerificationTestViewModel(_container, InstrumentQaRunTestManager));
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