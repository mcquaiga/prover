using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Prover.Application.Interfaces;
using Prover.Domain.EvcVerifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.Dashboard
{
    public class ValueDashboardViewModel : DashboardItemViewModel, IDashboardValueViewModel
    {
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();
        
        protected ValueDashboardViewModel(string title, string groupName, int sortOrder) : base(title, groupName, sortOrder) { }
        protected ValueDashboardViewModel(string title, string groupName) : base(title, groupName) { }
        protected ValueDashboardViewModel(string title, string groupName, IEntityDataCache<EvcVerificationTest> entityCache,  IObservable<Func<EvcVerificationTest, bool>> parentFilter = null) : base(title, groupName) { }

        public ValueDashboardViewModel(IEntityDataCache<EvcVerificationTest> entityCache, string title, string groupName, Func<EvcVerificationTest, bool> filter, IObservable<Func<EvcVerificationTest, bool>> parentFilter = null, int sortOrder = 99)
                : base(title, groupName, sortOrder)
        {
            var list = GenerateListStream(entityCache, parentFilter);
            
                list.CountChanged
                    //.Throttle(TimeSpan.FromMilliseconds(150))
                    .Select(i => list.Items.Count(filter))
                    .ToPropertyEx(this, model => model.Value, 0, scheduler: RxApp.MainThreadScheduler, deferSubscription: true).DisposeWith(Cleanup);
        }

        public extern int Value { [ObservableAsProperty] get; }

        //public IObservable<Func<EvcVerificationTest, bool>> ParentFilterObservable { get; }
        
    }
}