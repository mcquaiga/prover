using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Instruments;
using Prover.Core.VerificationTests;
using Prover.GUI.Events;
using Prover.GUI.Reporting;
using Prover.GUI.ViewModels.InstrumentViews;
using Prover.SerialProtocol;
using System;
using System.Windows;

namespace Prover.GUI.ViewModels.VerificationTestViews
{
    public class VerificationTestViewModel : InstrumentTestViewModel
    {
        public VerificationTestViewModel(IUnityContainer container, RotaryTestManager testManager) : base(container, testManager.Instrument)
        {
            _container.RegisterInstance(testManager);
            _container.Resolve<IEventAggregator>().Subscribe(this);

            if (testManager == null)
                throw new ArgumentNullException(nameof(testManager));
            InstrumentTestManager = testManager;

            SiteInformationItem = new InstrumentInfoViewModel(_container, InstrumentTestManager.Instrument);
            VolumeInformationItem = new VolumeVerificationViewModel(_container, InstrumentTestManager);
        }

        public RotaryTestManager InstrumentTestManager { get; set; }    

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
        #endregion
    }
}

