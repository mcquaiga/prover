using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using Prover.Application.Interfaces;
using Prover.Domain.EvcVerifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.Dashboard
{
    public class VerifiedCountsDashboardViewModel : DashboardItemViewModel
    {
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

        /// <inheritdoc />
        public VerifiedCountsDashboardViewModel(
                IEntityDataCache<EvcVerificationTest> entityCache, 
                string title, 
                string groupName, 
                IObservable<Func<EvcVerificationTest, bool>> parentFilter, 
                Func<EvcVerificationTest, bool> filter = null) : base(title, groupName)
        {
            filter = filter ?? (t => true);

            CountStream(test => test.Verified && filter.Invoke(test))
                    .ToPropertyEx(this, model => model.Passed, 0, scheduler: RxApp.MainThreadScheduler, deferSubscription: true)
                    .DisposeWith(Cleanup);

            CountStream(test => test.Verified == false && filter.Invoke(test))
                    .ToPropertyEx(this, model => model.Failed, 0, scheduler: RxApp.MainThreadScheduler, deferSubscription: true)
                    .DisposeWith(Cleanup);


            IObservable<int> CountStream(Func<EvcVerificationTest, bool> func)
            {
                return GenerateListStream(entityCache, parentFilter, func)
                       .CountChanged
                       .Throttle(TimeSpan.FromMilliseconds(150));
            }
        }

        public extern int Passed { [ObservableAsProperty] get; }
        public extern int Failed { [ObservableAsProperty] get; }
    }
}