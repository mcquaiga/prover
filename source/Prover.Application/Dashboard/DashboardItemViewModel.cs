using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using Prover.Application.Extensions;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Domain.EvcVerifications;
using Prover.Shared.Extensions;
using ReactiveUI;

namespace Prover.Application.Dashboard
{
    public abstract class DashboardItemViewModel : ReactiveObject, IDashboardItem, IDisposable
    {
        protected readonly CompositeDisposable Cleanup = new CompositeDisposable();

        protected DashboardItemViewModel(string title, string groupName, int sortOrder = 99)
        {
            Title = title;
            GroupName = groupName;
            SortOrder = sortOrder;
        }

        /// <inheritdoc />
        public string Title { get; }

        public string GroupName { get; }

        public int SortOrder { get; set; }

        protected IObservableList<EvcVerificationTest> GenerateListStream(IEntityDataCache<EvcVerificationTest> entityCache, IObservable<Func<EvcVerificationTest, bool>> parentFilter, Func<EvcVerificationTest, bool> filter = null)
        {
            filter = filter ?? (v => true);
            parentFilter = parentFilter ?? Observable.Return<Func<EvcVerificationTest, bool>>(test => true);

            return entityCache?
                   .Data(filter).Connect()
                   .Filter(parentFilter).AsObservableList();
                   //.Throttle(TimeSpan.FromMilliseconds(150));
        }
        
        protected IObservable<IChangeSet<EvcVerificationTest>> GenerateCacheStream(IEntityDataCache<EvcVerificationTest> entityCache, IObservable<Func<EvcVerificationTest, bool>> parentFilter, Func<EvcVerificationTest, bool> filter = null)
        {
            filter = filter ?? (v => true);
            parentFilter = parentFilter ?? Observable.Return<Func<EvcVerificationTest, bool>>(test => true);

            return entityCache?
                   .Data(filter).Connect()
                   .Filter(parentFilter);
                   //.Throttle(TimeSpan.FromMilliseconds(150));
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            Cleanup?.Dispose();
        }
    }
}