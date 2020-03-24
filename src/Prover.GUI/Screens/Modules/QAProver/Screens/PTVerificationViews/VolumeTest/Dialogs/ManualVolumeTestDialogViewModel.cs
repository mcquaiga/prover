using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.VerificationTests.VolumeVerification;
using Prover.GUI.Screens.Dialogs;
using ReactiveUI;

namespace Prover.GUI.Screens.Modules.QAProver.Screens.PTVerificationViews.VolumeTest.Dialogs
{
    public class ManualVolumeTestDialogViewModel : DialogViewModel
    {
        public VolumeTestManager TestManager { get; }

        public ManualVolumeTestDialogViewModel(ManualVolumeTestManager testManager, IEventAggregator eventAggregator,
            Func<CancellationToken, Task> taskFunc)
        {
            TestManager = testManager;

            //var preTestCommand =
            //    DialogDisplayHelpers.ProgressStatusDialogCommand(eventAggregator, "Starting Volume test...",
            //        ShowPrePostTestStatus);
            //TestManager.TestStepsObservable
            //    .Where(ts => ts == VolumeTestSteps.PreTest)
            //    .InvokeCommand(preTestCommand);

            //var postTestCommand =
            //    DialogDisplayHelpers.ProgressStatusDialogCommand(eventAggregator, "Finishing Volume test...",
            //        ShowPrePostTestStatus);
            //TestManager.TestStepsObservable
            //    .Where(ts => ts == VolumeTestSteps.PostTest)
            //    .InvokeCommand(postTestCommand);

            CancellationTokenSource = new CancellationTokenSource();
            TaskCommand = ReactiveCommand.CreateFromTask(() => taskFunc(CancellationTokenSource.Token));
            DoneCommand = ReactiveCommand.Create(() =>
            {
                TestManager.RunningTest = false;
                ShowDialog = false;
//                this.TryClose();
            });
            ShowDialog = true;
        }

        public ReactiveCommand<Unit, Unit> DoneCommand { get; set; }
        //public VolumeTestSteps CurrentTestStep { get; set; }

        //private async Task ShowPrePostTestStatus(IObserver<string> statusObserver, CancellationToken cancellationToken)
        //{
        //    await Task.Run(() =>
        //    {
        //        TestManager.StatusMessage.Subscribe(statusObserver);

        //        do
        //        {
        //        } while (CurrentTestStep == VolumeTestSteps.PreTest || CurrentTestStep == VolumeTestSteps.PostTest);
        //    }, cancellationToken);
        //}

        public override void Dispose()
        {
        }
    }
}