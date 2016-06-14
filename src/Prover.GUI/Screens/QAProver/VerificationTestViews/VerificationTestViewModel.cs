using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.Core.VerificationTests;
using Prover.GUI.Common.Events;
using Prover.GUI.Reports;

namespace Prover.GUI.Screens.QAProver.VerificationTestViews
{
    public class VerificationTestViewModel : InstrumentTestViewModel, IHandle<VerificationTestEvent>, IDisposable
    {
        public VerificationTestViewModel(IUnityContainer container, TestManager testManager)
            : base(container, testManager.Instrument)
        {
            _container.RegisterInstance(testManager);
            _container.Resolve<IEventAggregator>().Subscribe(this);

            if (testManager == null)
                throw new ArgumentNullException(nameof(testManager));
            InstrumentTestManager = testManager;

            QaTestRunViewItem = new QaTestRunViewModel(container, InstrumentTestManager.Instrument);
        }

        public QaTestRunViewModel QaTestRunViewItem { get; set; }

        public TestManager InstrumentTestManager { get; set; }

        public void Dispose()
        {
            InstrumentTestManager.Dispose();
        }

        public void Handle(VerificationTestEvent message)
        {
            Task.Run(async () => await SaveInstrument());
        }

        #region Methods

        public async Task SaveInstrument()
        {
            if (InstrumentTestManager == null) return;

            //if (!Instrument.HasPassed && MessageBox.Show("This instrument hasn't passed all tests." + Environment.NewLine + "Would you still like to save?", "Save", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            //    return;

            await InstrumentTestManager.SaveAsync();
            _container.Resolve<IEventAggregator>()
                .PublishOnBackgroundThread(new NotificationEvent("Successfully Saved instrument!"));
        }

        public void InstrumentReport()
        {
            var instrumentReport = new InstrumentGenerator(InstrumentTestManager.Instrument, _container);
            instrumentReport.Generate();
        }

        #endregion
    }
}