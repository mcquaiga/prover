using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Instruments;
using Prover.Core.VerificationTests;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;
using Prover.GUI.Reports;

namespace Prover.GUI.Screens.QAProver.VerificationTestViews
{
    public class QATestRunInteractiveViewModel : InstrumentTestViewModel, IHandle<VerificationTestEvent>, IDisposable
    {
        public QATestRunInteractiveViewModel(ScreenManager screenManager, IEventAggregator eventAggregator, QaRunTestManager qaRunTestManager, Instrument instrument) : base(screenManager, eventAggregator, instrument)
        {
            if (qaRunTestManager == null)
                throw new ArgumentNullException(nameof(qaRunTestManager));

            InstrumentQaRunTestManager = qaRunTestManager;

            //QaTestRunViewItem = new QaTestRunViewModel(InstrumentQaRunTestManager.Instrument);
        }

        //public QATestRunInteractiveViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
        //    : base(qaRunTestManager.Instrument)
        //{
        //    _container.RegisterInstance(qaRunTestManager);
        //    _container.Resolve<IEventAggregator>().Subscribe(this);


        //}

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
            EventAggregator
                .PublishOnBackgroundThread(new NotificationEvent("Successfully Saved instrument!"));
        }

        public void InstrumentReport()
        {
            //var instrumentReport = new InstrumentGenerator(InstrumentQaRunTestManager.Instrument, _container);
            //instrumentReport.Generate();
        }

        #endregion
    }
}