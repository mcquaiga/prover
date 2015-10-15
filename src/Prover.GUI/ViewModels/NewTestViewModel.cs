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

namespace Prover.GUI.ViewModels
{
    public class NewTestViewModel : ReactiveScreen
    {
        readonly IUnityContainer _container;

        public NewTestViewModel(IUnityContainer container)
        {
            _container = container;
            InstrumentCommPortName = SettingsManager.SettingsInstance.InstrumentCommPort;
            BaudRate = SettingsManager.SettingsInstance.InstrumentBaudRate;
            TachCommPortName = SettingsManager.SettingsInstance.TachCommPort;

            InstrumentManager = new InstrumentManager(_container, InstrumentCommPortName, BaudRate);
            if (TachCommPortName != null) InstrumentManager.SetupTachCommPort(TachCommPortName);
            base.NotifyOfPropertyChange(() => Instrument);

        }

        public InstrumentManager InstrumentManager { get; set; }
        
        public ICommPort CommPort { get; set; }
        public string InstrumentCommPortName { get; private set; }
        public string TachCommPortName { get; private set; }
        public BaudRateEnum BaudRate { get; private set; }

        public Instrument Instrument => InstrumentManager.Instrument;

        #region Methods

        public async void FetchInstrumentItems()
        {
            await Task.Run((Func<Task>)(async () =>
            {
                _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Starting download from instrument..."));
                if (this.InstrumentCommPortName == null)
                {
                    MessageBox.Show("Please select a Comm Port and Baud Rate first.", "Comm Port");
                    return;
                }
                
                await InstrumentManager.DownloadInfo();
                base.NotifyOfPropertyChange(() => Instrument);
                
                _container.Resolve<IEventAggregator>().PublishOnBackgroundThread(new NotificationEvent("Completed Download from Instrument!"));
                //Publish the change in instrument state to anyone who's listening
                _container.Resolve<IEventAggregator>().PublishOnUIThread(new InstrumentUpdateEvent(InstrumentManager));
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
        #endregion

        #region Views
        public SiteInformationViewModel SiteInformationItem => new SiteInformationViewModel(_container);
        public TemperatureViewModel TemperatureInformationItem => new TemperatureViewModel(_container);
        public VolumeViewModel VolumeInformationItem => new VolumeViewModel(_container);
        #endregion
    }
}

