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
        public VerificationTestViewModel(IUnityContainer container, QaRunTestManager qaRunTestManager)
            : base(container, qaRunTestManager.Instrument)
        {
            _container.RegisterInstance(qaRunTestManager);
            _container.Resolve<IEventAggregator>().Subscribe(this);

            if (qaRunTestManager == null)
                throw new ArgumentNullException(nameof(qaRunTestManager));

            InstrumentQaRunTestManager = qaRunTestManager;

            QaTestRunViewItem = new QaTestRunViewModel(container, InstrumentQaRunTestManager.Instrument);
        }

        public QaTestRunViewModel QaTestRunViewItem { get; set; }

        public QaRunTestManager InstrumentQaRunTestManager { get; set; }

        public void Dispose()
        {
            InstrumentQaRunTestManager.Dispose();
        }

        public void Handle(VerificationTestEvent message)
        {
            Task.Run(async () => await SaveInstrument());
        }

        #region Methods

        public async Task SaveInstrument()
        {
            if (InstrumentQaRunTestManager == null) return;

            //if (!Instrument.HasPassed && MessageBox.Show("This instrument hasn't passed all tests." + Environment.NewLine + "Would you still like to save?", "Save", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            //    return;

            await InstrumentQaRunTestManager.SaveAsync();
            _container.Resolve<IEventAggregator>()
                .PublishOnBackgroundThread(new NotificationEvent("Successfully Saved instrument!"));
        }

        public void InstrumentReport()
        {
            var instrumentReport = new InstrumentGenerator(InstrumentQaRunTestManager.Instrument, _container);
            instrumentReport.Generate();
        }

        #endregion
    }
}