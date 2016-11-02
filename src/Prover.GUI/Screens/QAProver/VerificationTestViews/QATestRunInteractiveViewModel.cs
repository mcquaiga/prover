using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.Core.DriveTypes;
using Prover.Core.VerificationTests;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;
using Prover.GUI.Common.Screens;
using Prover.GUI.Reports;

namespace Prover.GUI.Screens.QAProver.VerificationTestViews
{
    public class QaTestRunInteractiveViewModel : ViewModelBase, IHandle<VerificationTestEvent>, IDisposable
    {
        private readonly InstrumentReportGenerator _reportGenerator;

        public QaTestRunInteractiveViewModel(ScreenManager screenManager, IEventAggregator eventAggregator,
            IQaRunTestManager qaRunTestManager, InstrumentReportGenerator reportGenerator)
            : base(screenManager, eventAggregator)
        {
            _reportGenerator = reportGenerator;
            QaRunTestManager = qaRunTestManager;

            QaTestRunViewItem = ScreenManager.ResolveViewModel<QaTestRunViewModel>();
        }

        public QaTestRunViewModel QaTestRunViewItem { get; set; }

        public IQaRunTestManager QaRunTestManager { get; set; }

        public void Dispose()
        {
            QaRunTestManager.Dispose();
        }

        public void Handle(VerificationTestEvent message)
        {
            throw new NotImplementedException();
        }

        public async Task Initialize(InstrumentType instrumentType, IDriveType driveType)
        {
            await QaRunTestManager.InitializeTest(instrumentType, driveType);
            await QaTestRunViewItem.Initialize(QaRunTestManager.Instrument);
        }

        public void InstrumentReport()
        {
            _reportGenerator.GenerateAndViewReport(QaRunTestManager.Instrument);
        }
    }
}