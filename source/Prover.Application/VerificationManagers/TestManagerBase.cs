using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using ReactiveUI;

namespace Prover.Application.VerificationManagers
{
    public abstract class TestManagerBase : RoutableViewModelBase
    {
        public ReactiveCommand<Unit, bool> SaveCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> PrintTestReport { get; protected set; }
        public ReactiveCommand<Unit, Unit> SubmitTest { get; protected set; }
        public EvcVerificationViewModel TestViewModel { get; protected set; }

        protected TestManagerBase(ILogger<TestManagerBase> logger,
            IScreenManager screenManager,
            IVerificationTestService verificationService,
            EvcVerificationViewModel testViewModel,
            string urlSegment = null) : base(screenManager, urlSegment ?? "VerificationsManager")
        {
            TestViewModel = testViewModel;
            SaveCommand = ReactiveCommand.CreateFromTask(async () => {
                logger.LogDebug("Saving test...");

                var updated = await verificationService.AddOrUpdate(TestViewModel);
                if (updated != null)
                {
                    logger.LogDebug("Saved test successfully");
                    await NotificationInteractions.SnackBarMessage.Handle("SAVED");
                    return true;
                }
                return false;
            }).DisposeWith(Cleanup);
            SaveCommand.ThrownExceptions.Subscribe();

            var canSubmit = this.WhenAnyObservable(x => x.TestViewModel.VerifiedObservable).ObserveOn(RxApp.MainThreadScheduler);
            SubmitTest = ReactiveCommand.CreateFromTask(async () => {

                if (true)
                {
                    TestViewModel.TestDateTime = DateTime.Now;

                    await SaveCommand.Execute();
                    (TestViewModel as IDisposable)?.Dispose();
                    await ScreenManager.GoHome();
                }
            }, canSubmit).DisposeWith(Cleanup);

            PrintTestReport = ReactiveCommand.CreateFromObservable(() => MessageInteractions.ShowMessage.Handle("Verifications Report feature not yet implemented.")).DisposeWith(Cleanup);

            this.WhenAnyObservable(x => x.TestViewModel.VerifiedObservable)
                .Where(v => v)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(async x => await NotificationInteractions.SnackBarMessage.Handle("Verification Complete"))
                .Subscribe()
                .DisposeWith(Cleanup);
        }
    }
}