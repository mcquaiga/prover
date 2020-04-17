using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.ViewModels;
using ReactiveUI;

namespace Prover.Application.Dashboard
{
    public abstract class DashboardItemViewModel : ViewModelBase, IDashboardItem, IDisposable
    {

        protected DashboardItemViewModel(){}

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

        protected IObservableList<EvcVerificationTest> ListStreamInstance;

        protected IObservableList<EvcVerificationTest> GenerateListStream(IEntityDataCache<EvcVerificationTest> entityCache, IObservable<Func<EvcVerificationTest, bool>> parentFilter)
        {
            //filter = filter ?? (v => true);
            if (ListStreamInstance != null)
            {
                return ListStreamInstance;
            }

            parentFilter = parentFilter ?? Observable.Return<Func<EvcVerificationTest, bool>>(test => true);

            return entityCache?.Data()
                                            .Connect()
                                            .ObserveOn(RxApp.MainThreadScheduler)
                                            .Throttle(TimeSpan.FromMilliseconds(50))
                                            .Filter(parentFilter)
                                            //.DelaySubscription(TimeSpan.FromSeconds(2))
                                            .AsObservableList()
                                            .DisposeWith(Cleanup);

        }
        
        protected IObservable<IChangeSet<EvcVerificationTest>> GenerateCacheStream(IEntityDataCache<EvcVerificationTest> entityCache, IObservable<Func<EvcVerificationTest, bool>> parentFilter)
        {
            //filter = filter ?? (v => true);
            parentFilter = parentFilter ?? Observable.Empty<Func<EvcVerificationTest, bool>>(test => true);

            return entityCache?.Data().Connect()
                              .Filter(parentFilter)
                              .Throttle(TimeSpan.FromMilliseconds(50))
                              .ObserveOn(RxApp.MainThreadScheduler)
                              .DisposeMany();
                   //;
        }
    }
}