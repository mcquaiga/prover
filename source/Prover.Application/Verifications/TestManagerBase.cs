using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Prover.Application.Interactions;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using ReactiveUI;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;

namespace Prover.Application.Verifications {
	public interface IViewModelNavigationEvents {
		bool CanNavigateAway();
	}

	public interface IRoutableLifetimeHandler {
		CancellationToken OnChanging { get; set; }
		IObservable<bool> CanChange { get; set; }
		Task ChangingRoute(IObserver<bool> canChange);
	}

	public abstract class TestManagerBase : ViewModelBase, IQaTestRunManager, IActivatableViewModel, IRoutableLifetimeHandler {
		private CancellationToken _onChanging;
		protected ILogger<TestManagerBase> Logger { get; }
		protected IScreenManager ScreenManager { get; }
		protected IVerificationService VerificationService { get; }

		protected TestManagerBase(ILogger<TestManagerBase> logger,
									IScreenManager screenManager,
									IVerificationService verificationService,
									EvcVerificationViewModel testViewModel) {
			var cts = new CancellationTokenSource();

			Logger = logger;
			ScreenManager = screenManager;
			HostScreen = screenManager;
			VerificationService = verificationService;

			TestViewModel = testViewModel;

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
		protected override void HandleActivation(CompositeDisposable cleanup) {
			SaveCommand = ReactiveCommand.CreateFromTask(async () => {
				Logger.LogDebug("Saving test...");
				var updated = await VerificationService.Save(TestViewModel);

				if (updated != null) {
					Logger.LogDebug("Saved test successfully");
					await Notifications.SnackBarMessage.Handle("SAVED");
					return true;
				}

				return false;
			})
										 .DisposeWith(cleanup);
			SaveCommand.ThrownExceptions.Subscribe().DisposeWith(cleanup);


			var canSubmit = this.WhenAnyObservable(x => x.TestViewModel.VerifiedObservable)
								.ObserveOn(RxApp.MainThreadScheduler);

			SubmitTest = ReactiveCommand.CreateFromTask(async () => {
				if (true) {
					SuppressChangeNotifications().DisposeWith(Cleanup);
					await VerificationEvents.TestEvents<IQaTestRunManager>.OnComplete.Publish(this);
					await VerificationService.CompleteVerification(TestViewModel);
					await ScreenManager.GoHome();
				}
			}, canSubmit)
										.DisposeWith(cleanup);
			SubmitTest.ThrownExceptions
					  .Subscribe(ex => Exceptions.Error.Handle($"An error occured submitting verification. {ex.Message}"))
					  .DisposeWith(cleanup);

			PrintTestReport = ReactiveCommand.CreateFromObservable(() =>
					Messages.ShowMessage.Handle("Verifications Report feature not yet implemented."))
											 .DisposeWith(cleanup);
		}


		/// <inheritdoc />
		public IObservable<bool> CanChange { get; set; }

		/// <inheritdoc />
		public async Task ChangingRoute(IObserver<bool> canChange) {
			if (await HasUnsavedChanges.LastOrDefaultAsync()) {

			}
		}
	}
}