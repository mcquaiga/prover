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
using Prover.Core.VerificationTests;

namespace Prover.GUI.ViewModels
{
    public class NewTestViewModel : ReactiveScreen,  IHandle<InstrumentUpdateEvent>
    {
        readonly IUnityContainer _container;

        public NewTestViewModel(IUnityContainer container, TestManager testManager)
        {
            _container = container;
            _container.RegisterInstance(testManager);
            _container.Resolve<IEventAggregator>().Subscribe(this);

            if (testManager == null)
                throw new ArgumentNullException(nameof(testManager));   
            InstrumentTestManager = testManager;
        }

        public TestManager InstrumentTestManager { get; set; }        
        public ICommPort CommPort { get; set; }
        public string InstrumentCommPortName { get; private set; }
        public string TachCommPortName { get; private set; }
        public BaudRateEnum BaudRate { get; private set; }

        public Instrument Instrument => InstrumentTestManager.Instrument;

        #region Views
        public SiteInformationViewModel SiteInformationItem => new SiteInformationViewModel(_container, InstrumentTestManager);
        public PTVerificationViewModel PressureTemperatureVerificationSection => new PTVerificationViewModel(_container, InstrumentTestManager);
        public TemperatureViewModel TemperatureInformationItem { get; private set; }
        public PressureViewModel PressureInformationItem { get; private set; }
        public VolumeViewModel VolumeInformationItem => new VolumeViewModel(_container, InstrumentTestManager);
        #endregion

        #region Methods
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

        public void Handle(InstrumentUpdateEvent message)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

