using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.VerificationTests.VolumeVerification;
using Prover.GUI.Common.Screens.Dialogs;
using ReactiveUI;

namespace Prover.GUI.Modules.QAProver.Screens.PTVerificationViews.VolumeTest.Dialogs
{
    public class ManualVolumeTestDialogViewModel : DialogViewModel
    {
        public ManualVolumeTestManager TestManager { get; }

        public ManualVolumeTestDialogViewModel(ManualVolumeTestManager testManager, IEventAggregator eventAggregator, Func<CancellationToken, Task> taskFunc)
        {
            TestManager = testManager;

            TestManager.TestStepsObservable
                .Subscribe(ts => CurrentTestStep = ts);

            var preTestCommand =
                DialogDisplayHelpers.ProgressStatusDialogCommand(eventAggregator, "Starting Volume test...",
                    ShowPrePostTestStatus);
            TestManager.TestStepsObservable
                .Where(ts => ts == VolumeTestSteps.PreTest)
                .InvokeCommand(preTestCommand);

            var postTestCommand =
                DialogDisplayHelpers.ProgressStatusDialogCommand(eventAggregator, "Finishing Volume test...",
                    ShowPrePostTestStatus);
            TestManager.TestStepsObservable
                .Where(ts => ts == VolumeTestSteps.PostTest)
                .InvokeCommand(postTestCommand);

            CancellationTokenSource = new CancellationTokenSource();
            TaskCommand = ReactiveCommand.CreateFromTask(() => taskFunc(CancellationTokenSource.Token));
        }

        public VolumeTestSteps CurrentTestStep { get; set; }

        private async Task ShowPrePostTestStatus(IObserver<string> statusObserver, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                TestManager.StatusMessage.Subscribe(statusObserver);

                do
                {

                } while (CurrentTestStep == VolumeTestSteps.PreTest || CurrentTestStep == VolumeTestSteps.PostTest);

            }, cancellationToken);
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
