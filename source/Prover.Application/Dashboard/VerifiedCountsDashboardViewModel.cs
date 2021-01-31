using DynamicData;
using Prover.Application.Models.EvcVerifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Prover.Application.Dashboard
{
	public class VerifiedCountsDashboardViewModel : DashboardItemViewModel
	{
		/// <inheritdoc />
		public VerifiedCountsDashboardViewModel(string title, string groupName, IObservableCache<EvcVerificationTest, Guid> itemsCache, Func<EvcVerificationTest, bool> filter = null)
			: base(title, groupName)
		{
			filter = filter ?? (t => true);

			var shared = itemsCache.Connect(filter).QueryWhenChanged();

			shared.Select(x => x.Items.Count(test => test.Verified))
				  .ToPropertyEx(this, model => model.Passed, 0, scheduler: RxApp.MainThreadScheduler, deferSubscription: true)
				  .DisposeWith(DeactivateDisposer);

			shared.Select(x => x.Items.Count(test => test.Verified == false))
				  .ToPropertyEx(this, model => model.Failed, 0, scheduler: RxApp.MainThreadScheduler, deferSubscription: true)
				  .DisposeWith(DeactivateDisposer);

			this.WhenAnyValue(x => x.Passed, x => x.Failed, (p, f) => p + f)
				.ToPropertyEx(this, x => x.Total)
				.DisposeWith(DeactivateDisposer);
		}

		public extern int Passed { [ObservableAsProperty] get; }
		public extern int Failed { [ObservableAsProperty] get; }
		public extern int Total { [ObservableAsProperty] get; }

		/// <inheritdoc />
		protected override void HandleActivation(CompositeDisposable cleanup)
		{

		}
	}
}

//public VerifiedCountsDashboardViewModel(string title, IObservableList<EvcVerificationTest> data, string groupName, Func<EvcVerificationTest, bool> filter = null)
//    : base(title, groupName)
//{
//    var changes = data.Connect();

//    var filteredData = changes.Filter(filter.Invoke).AsObservableList();

//    changes.CountChanged()
//           .Filter(x => x.Verified)
//           .Select(_ => Passed + 1)
//           .ToPropertyEx(this, 
//                   model => model.Passed, 
//                   initialValue: filteredData.Items.Count(v => v.Verified), 
//                   scheduler: RxApp.MainThreadScheduler, 
//                   deferSubscription: true)
//           .DisposeWith(Cleanup);

//    filteredData.CountChanged
//                .Select(_ => filteredData.Items.Count(v => v.Verified == false))
//                .ToPropertyEx(this, model => model.Failed, 0, scheduler: RxApp.MainThreadScheduler, deferSubscription: true)
//                .DisposeWith(Cleanup);

//}