using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Communication;
using Prover.Core.Models.Instruments;
using Prover.GUI.Events;
using Prover.GUI.Properties;
using Prover.GUI.ViewModels.TemperatureViews;
using Prover.GUI.Views;
using Prover.GUI.Views.TemperatureViews;
using Prover.SerialProtocol;
using ReactiveUI;
using Prover.Core.Settings;
using System.Threading;
using Prover.Core.Events;
using Prover.GUI.Reporting;
using Prover.GUI.ViewModels.PressureViews;
using Prover.GUI.ViewModels.PTVerificationViews;
using MaterialDesignThemes.Wpf;

namespace Prover.GUI.ViewModels
{
    public class NewTestViewModel : ReactiveScreen, IHandle<SettingsChangeEvent>
    {
        readonly IUnityContainer _container;

        public NewTestViewModel(IUnityContainer container)
        {
            _container = container;
            _container.Resolve<IEventAggregator>().Subscribe(this);
        }

        public TestManager InstrumentTestManager { get; set; }        
        public ICommPort CommPort { get; set; }
        public string InstrumentCommPortName { get; private set; }
        public string TachCommPortName { get; private set; }
        public BaudRateEnum BaudRate { get; private set; }

        public Instrument Instrument => InstrumentTestManager.Instrument;

        #region Views
        public SiteInformationViewModel SiteInformationItem => new SiteInformationViewModel(_container);
        public PTVerificationViewModel PressureTemperatureVerificationSection { get; set; }
        public TemperatureViewModel TemperatureInformationItem { get; private set; }
        public PressureViewModel PressureInformationItem { get; private set; }
        public VolumeViewModel VolumeInformationItem => new VolumeViewModel(_container);
        #endregion

        #region Methods
        private void SetupTestManager()
        {
            VerifySettings();

            if (InstrumentTestManager == null)
            {
                var commPort = Communications.CreateCommPortObject(InstrumentCommPortName, BaudRate);
                InstrumentTestManager = new TestManager(_container, commPort, TachCommPortName);
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

            base.NotifyOfPropertyChange(() => InstrumentCommPortName);
            base.NotifyOfPropertyChange(() => BaudRate);
            base.NotifyOfPropertyChange(() => TachCommPortName);

            if (string.IsNullOrEmpty(InstrumentCommPortName))
            {
                _container.Resolve<IWindowManager>().ShowDialog(new SettingsViewModel(_container), null, SettingsViewModel.WindowSettings);
            }
        }

        public async void InitializeTest()
        {
            await Task.Run((Func<Task>)(async () =>
            {
                _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Starting download from instrument..."));

                try
                {
                    SetupTestManager();
                    await InstrumentTestManager.InitializeInstrument(InstrumentType.MiniMax);

                    base.NotifyOfPropertyChange(() => Instrument);

                    BuildViews();

                    _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Completed Download from Instrument!"));
                    _container.Resolve<IEventAggregator>().PublishOnUIThread(new InstrumentUpdateEvent(InstrumentTestManager));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured communicating with the instrument." + Environment.NewLine
                        + ex.Message,
                        "Error",
                        MessageBoxButton.OK);
                }
            }));          
        }

        private void BuildViews()
        {
            PressureTemperatureVerificationSection = new PTVerificationViewModel(_container);
            base.NotifyOfPropertyChange(() => PressureTemperatureVerificationSection);

            if (Instrument.Temperature != null)
            {
                TemperatureInformationItem = new TemperatureViewModel(_container);
                base.NotifyOfPropertyChange(() => TemperatureInformationItem);
            }

            if (Instrument.Pressure != null)
            {
                PressureInformationItem = new PressureViewModel(_container);
                base.NotifyOfPropertyChange(() => PressureInformationItem);
            }
               
        }

        public async void SaveInstrument()
        {
            if (InstrumentTestManager == null) return;

            if (!Instrument.HasPassed && MessageBox.Show("This instrument hasn't passed all tests." + Environment.NewLine + "Would you still like to save?", "Save", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            await InstrumentTestManager.SaveAsync();
            _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Successfully Saved instrument!"));
        }

        public void InstrumentReport()
        {
            var instrumentReport = new InstrumentGenerator(InstrumentTestManager.Instrument, _container);
            instrumentReport.Generate();
        }

        public void Handle(SettingsChangeEvent message)
        {
            SetupTestManager();
        }
        #endregion
    }
}

