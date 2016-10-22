using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Models.Instruments;
using Prover.Core.VerificationTests;
using Prover.GUI.Common;
using Prover.GUI.Common.Events;
using Prover.GUI.Common.Screens;

namespace Prover.GUI.Screens.QAProver.VerificationTestViews
{
    //public class QaTestRunInteractiveViewModel : ViewModelBase, IHandle<VerificationTestEvent>, IDisposable
    //{
    //    public QaTestRunInteractiveViewModel(ScreenManager screenManager, IEventAggregator eventAggregator)
    //        : base(screenManager, eventAggregator)
    //    {
    //        if (qaRunTestManager == null)
    //            throw new ArgumentNullException(nameof(qaRunTestManager));

    //        QaRunTestManager = qaRunTestManager;

    //        QaTestRunViewItem = ScreenManager.ResolveViewModel<QaTestRunViewModel>();
    //    }

    //    public QaTestRunViewModel QaTestRunViewItem { get; set; }

    //    public IQaRunTestManager QaRunTestManager { get; set; }

    //    public void InstrumentReport()
    //    {
    //        //var instrumentReport = new InstrumentGenerator(InstrumentQaRunTestManager.Instrument, _container);
    //        //instrumentReport.Generate();
    //    }

    //    public void Dispose()
    //    {
    //        QaRunTestManager.Dispose();
    //    }

    //}
}