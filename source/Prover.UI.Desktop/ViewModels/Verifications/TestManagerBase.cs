using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.Verifications;
using Prover.Application.ViewModels;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Prover.UI.Desktop.ViewModels.Verifications
{
	public abstract class TestManagerBase : ViewModelWpfBase, IHaveToolbarItems, IQaTestRunManager
	{
		protected ILogger<ManualTestManager> Logger { get; }
		protected IScreenManager ScreenManager { get; }
		protected IVerificationTestService VerificationService { get; }


		protected TestManagerBase(ILogger<ManualTestManager> logger,
									IScreenManager screenManager,
									IVerificationTestService verificationService,
									EvcVerificationViewModel testViewModel)
		{
			Logger = logger;
			ScreenManager = screenManager;
			VerificationService = verificationService;

			TestViewModel = testViewModel;

			SaveCommand = ReactiveCommand.CreateFromTask(async () =>
										 {
											 logger.LogDebug("Saving test...");
											 var updated = await verificationService.Save(TestViewModel);

											 if (updated != null)
											 {
												 logger.LogDebug("Saved test successfully");
												 await Notifications.SnackBarMessage.Handle("SAVED");
												 return true;
											 }

											 return false;
										 })
										 .DisposeWith(Cleanup);
			SaveCommand.ThrownExceptions.Subscribe();

			//SaveCommand.Do(x => VerificationEvents.OnSave.Publish(TestViewModel.ToModel()))
			//           .Subscribe().DisposeWith(Cleanup);
			var canSubmit = this.WhenAnyObservable(x => x.TestViewModel.VerifiedObservable).ObserveOn(RxApp.MainThreadScheduler);

			SubmitTest = ReactiveCommand.CreateFromTask(async () =>
										{
											if (true)
											{
												SuppressChangeNotifications().DisposeWith(Cleanup);
												await VerificationEvents.TestEvents<IQaTestRunManager>.OnComplete.Publish(this);
												await verificationService.SubmitVerification(TestViewModel);
												await screenManager.GoHome();
											}
										}, canSubmit)
										.DisposeWith(Cleanup);
			SubmitTest.ThrownExceptions.Subscribe(ex => Exceptions.Error.Handle($"An error occured submitting verification. {ex.Message}"));
			PrintTestReport = ReactiveCommand.CreateFromObservable(() => Messages.ShowMessage.Handle("Verifications Report feature not yet implemented.")).DisposeWith(Cleanup);

			//this.WhenAnyObservable(x => x.TestViewModel.VerifiedObservable)
			//	.Where(v => v)
			//	.ObserveOn(RxApp.MainThreadScheduler)
			//	.Do(async x => await Notifications.ActionMessage.Handle("Submit verified test?"))
			//	.Subscribe()
			//	.DisposeWith(Cleanup);
			AddToolbarItem(SaveCommand, PackIconKind.ContentSave);
			AddToolbarItem(SubmitTest, PackIconKind.Send);
			AddToolbarItem(PrintTestReport, PackIconKind.PrintPreview);
		}

		public ReactiveCommand<Unit, bool> SaveCommand { get; protected set; }
		public ReactiveCommand<Unit, Unit> PrintTestReport { get; protected set; }
		public ReactiveCommand<Unit, Unit> SubmitTest { get; protected set; }

		/// <inheritdoc />
		public EvcVerificationViewModel TestViewModel { get; set; }

		//protected void Initialize(ILogger<TestManagerBase> logger, IScreenManager screenManager, IVerificationTestService verificationService, EvcVerificationViewModel testViewModel, string urlSegment = null
		//) //: base(screenManager, urlSegment ?? "VerificationManager")
		//{

		//}
	}
}