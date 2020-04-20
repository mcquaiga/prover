using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.Verifications;
using Prover.Application.ViewModels;
using ReactiveUI;

namespace Prover.UI.Desktop.ViewModels.Verifications
{
    public abstract class TestManagerBase : ViewModelWpfBase, IHaveToolbarItems, IQaTestRunManager
    {
        protected TestManagerBase(){}

        protected TestManagerBase(ILogger<TestManagerBase> logger, IScreenManager screenManager)
        {
           
        }

        protected void Initialize(ILogger<TestManagerBase> logger,
                IScreenManager screenManager,
                IVerificationTestService verificationService,
                EvcVerificationViewModel testViewModel,
                string urlSegment = null) //: base(screenManager, urlSegment ?? "VerificationManager")
        {
            TestViewModel = testViewModel;

            SaveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
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
            SubmitTest = ReactiveCommand.CreateFromTask(async () =>
            {
                if (true)
                {
                    TestViewModel.SubmittedDateTime = DateTime.Now;
                    await SaveCommand.Execute();

                    await VerificationEvents.TestEvents<IQaTestRunManager>.OnComplete.Publish(this);

                    (TestViewModel as IDisposable)?.Dispose();
                    await screenManager.GoHome();
                }
            }, canSubmit).DisposeWith(Cleanup);

            PrintTestReport = ReactiveCommand.CreateFromObservable(() => MessageInteractions.ShowMessage.Handle("Verifications Report feature not yet implemented.")).DisposeWith(Cleanup);

            this.WhenAnyObservable(x => x.TestViewModel.VerifiedObservable)
                .Where(v => v)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(async x => await NotificationInteractions.ActionMessage.Handle("Submit verified test?"))
                .Subscribe()
                .DisposeWith(Cleanup);

            AddToolbarItem(SaveCommand, PackIconKind.ContentSave);
            AddToolbarItem(SubmitTest, PackIconKind.Send);
            AddToolbarItem(PrintTestReport, PackIconKind.PrintPreview);
        }

        public ReactiveCommand<Unit, bool> SaveCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> PrintTestReport { get; protected set; }
        public ReactiveCommand<Unit, Unit> SubmitTest { get; protected set; }

        /// <inheritdoc />
        public EvcVerificationViewModel TestViewModel { get; set; }
    }
}