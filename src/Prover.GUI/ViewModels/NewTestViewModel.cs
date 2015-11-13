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

        public InstrumentManager InstrumentManager { get; set; }        
        public ICommPort CommPort { get; set; }
        public string InstrumentCommPortName { get; private set; }
        public string TachCommPortName { get; private set; }
        public BaudRateEnum BaudRate { get; private set; }

        public Instrument Instrument => InstrumentManager.Instrument;

        #region Methods
        private void SetupInstrument()
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
            else
            {
                InstrumentManager = new InstrumentManager(_container, InstrumentCommPortName, BaudRate);
                if (!string.IsNullOrEmpty(TachCommPortName)) InstrumentManager.SetupTachCommPort(TachCommPortName);
                base.NotifyOfPropertyChange(() => Instrument);
            }
        }

        protected override void OnViewReady(object view)
        {
            base.OnViewReady(view);
            SetupInstrument();
        }

        public async void FetchInstrumentItems()
        {
            await Task.Run((Func<Task>)(async () =>
            {
                if (InstrumentManager == null) SetupInstrument();
                _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Starting download from instrument..."));
                if (this.InstrumentCommPortName == null)
                {
                    MessageBox.Show("Please select a Comm Port and Baud Rate first.", "Comm Port");
                    return;
                }

                try
                {
                    await InstrumentManager.DownloadInfo();
                    base.NotifyOfPropertyChange(() => Instrument);
                    _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Completed Download from Instrument!"));
                    //Publish the change in instrument state to anyone who's listening
                    _container.Resolve<IEventAggregator>().PublishOnUIThread(new InstrumentUpdateEvent(InstrumentManager));
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

        public async void SaveInstrument()
        {
            if (InstrumentManager == null) return;

            if (!Instrument.HasPassed && MessageBox.Show("This instrument hasn't passed all tests." + Environment.NewLine + "Would you still like to save?", "Save", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;

            await InstrumentManager.SaveAsync();
            _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Successfully Saved instrument!"));
        }

        public void InstrumentReport()
        {
            var instrumentReport = new InstrumentGenerator(InstrumentManager.Instrument, _container);
            instrumentReport.Generate();
        }

        public void Handle(SettingsChangeEvent message)
        {
            SetupInstrument();
        }
        #endregion

        #region Views
        public SiteInformationViewModel SiteInformationItem => new SiteInformationViewModel(_container);
        public TemperatureViewModel TemperatureInformationItem => new TemperatureViewModel(_container);
        public VolumeViewModel VolumeInformationItem => new VolumeViewModel(_container);
        #endregion
    }
}

