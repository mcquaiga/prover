using DynamicData;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.ViewModels;
using System;
using System.Reactive.Linq;

namespace Prover.Application.Dashboard
{
	public abstract class DashboardItemViewModel : ViewModelBase, IDashboardItem, IDisposable
	{

		protected DashboardItemViewModel() { }

		protected DashboardItemViewModel(string title = null, string groupName = null, int sortOrder = 99)
		{
			Title = title;
			GroupName = groupName;
			SortOrder = sortOrder;
		}

		/// <inheritdoc />
		public string Title { get; protected set; }

		public string GroupName { get; protected set; }

		public int SortOrder { get; set; }

		protected IObservableList<EvcVerificationTest> ListStream;

		protected IObservableList<EvcVerificationTest> GenerateListStream(IEntityCache<EvcVerificationTest> entityCache, IObservable<Func<EvcVerificationTest, bool>> parentFilter)
		{

			parentFilter = parentFilter ?? Observable.Return<Func<EvcVerificationTest, bool>>(test => true);
			ListStream = GenerateCacheStream(entityCache, parentFilter).Connect().RemoveKey().AsObservableList();

			return ListStream;
		}

		protected IObservableCache<EvcVerificationTest, Guid> GenerateCacheStream(IEntityCache<EvcVerificationTest> entityCache, IObservable<Func<EvcVerificationTest, bool>> parentFilter)
		{
			//filter = filter ?? (v => true);
			parentFilter = parentFilter ?? Observable.Empty<Func<EvcVerificationTest, bool>>(test => true);

			return entityCache?.Data.Connect().Filter(parentFilter).Throttle(TimeSpan.FromMilliseconds(50)).AsObservableCache();
			//;
		}
	}
}