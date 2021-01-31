using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using Prover.Application.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels.Corrections
{
	public abstract class VerificationViewModel : VerifyViewModel, IAssertVerification, IActivatableViewModel
	{
		protected VerificationViewModel()
		{
			VerifiedObservable = Observable.Empty(false);
		}

		public virtual extern bool Verified { [ObservableAsProperty] get; }

		public IObservable<bool> VerifiedObservable { get; protected set; }

		protected void RegisterVerificationsForVerified(ICollection<VerificationViewModel> verifications)
		{
			if (verifications == null || !verifications.Any())
				return;

			VerifiedObservable = verifications.AsObservableChangeSet()
											  .AutoRefresh(model => model.Verified)
											  .ToCollection()
											  .Select(x => x.Any() && x.All(y => y != null && y.Verified))
											  //.LogDebug(x => $"{GetType().Name} - Verified = {x}", Logger)
											  .LogErrors(Logger)
											  .LoggedCatch(this, VerifiedObservable)
											  .ObserveOn(RxApp.MainThreadScheduler);
		}

		protected override void HandleActivation(CompositeDisposable cleanup)
		{
			this.WhenAnyObservable(x => x.VerifiedObservable)
				.Throttle(TimeSpan.FromMilliseconds(25), RxApp.TaskpoolScheduler)
				.ToPropertyEx(this, x => x.Verified, deferSubscription: true, scheduler: RxApp.MainThreadScheduler)
				.DisposeWith(Cleanup);
		}

		protected override void HandleDeactivation()
		{

		}

	}
}