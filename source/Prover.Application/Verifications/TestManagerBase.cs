using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using ReactiveUI;

namespace Prover.Application.Verifications
{
	public interface IViewModelNavigationEvents
	{
		bool CanNavigateAway();
	}

	public abstract class TestManagerBase : ViewModelBase, IQaTestRunManager, IActivatableViewModel, IViewModelNavigationEvents
	{
		protected ILogger<TestManagerBase> Logger { get; }
		protected IScreenManager ScreenManager { get; }
		protected IVerificationService VerificationService { get; }

		protected TestManagerBase(ILogger<TestManagerBase> logger,
									IScreenManager screenManager,
									IVerificationService verificationService,
									EvcVerificationViewModel testViewModel)
		{
			var cts = new CancellationTokenSource();


			Logger = logger;
			ScreenManager = screenManager;
			HostScreen = screenManager;
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
												await verificationService.CompleteVerification(TestViewModel);
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

		}

		public ReactiveCommand<Unit, bool> SaveCommand { get; protected set; }
		public ReactiveCommand<Unit, Unit> PrintTestReport { get; protected set; }
		public ReactiveCommand<Unit, Unit> SubmitTest { get; protected set; }

		/// <inheritdoc />
		public IDeviceSessionManager DeviceManager { get; protected set; }

		public IObservable<bool> HasUnsavedChanges { get; protected set; }
		/// <inheritdoc />
		public EvcVerificationViewModel TestViewModel { get; protected set; }

		//protected void Initialize(ILogger<TestManagerBase> logger, IScreenManager screenManager, IVerificationTestService verificationService, EvcVerificationViewModel testViewModel, string urlSegment = null
		//) //: base(screenManager, urlSegment ?? "VerificationManager")
		//{

		//}
		/// <inheritdoc />
		public string UrlPathSegment { get; } = "TestManager";

		/// <inheritdoc />
		public IScreen HostScreen { get; }

		/// <inheritdoc />
		public ViewModelActivator Activator { get; } = new ViewModelActivator();

		/// <inheritdoc />
		public bool CanNavigateAway()
		{
			var canChange = true;

			if (!TestViewModel.Verified && )

				return CanNavigateAway(canChange);
		}

		protected virtual bool CanNavigateAway(bool canNavigateAway)
		{
			return canNavigateAway;
		}
	}
}