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
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.Dashboard
{
    public interface IDashboardValueViewModel : IDashboardItem
    {
        int Value { get; }
    }

    public class ValueDashboardViewModel : ReactiveObject, IDashboardValueViewModel, IDisposable
    {
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();
        
        public ValueDashboardViewModel(IEntityDataCache<EvcVerificationTest> entityCache, string title, Func<EvcVerificationTest, bool> filter = null)
        {
            Title = title;

            filter = filter ?? (v => v.TestDateTime.BetweenThenAndNow(DateTime.Now.StartOfWeek()));
            
            (entityCache as VerificationTestService)?
                    .Data(filter).CountChanged
                    .Throttle(TimeSpan.FromMilliseconds(150))
                    .LogDebug(x => $"Count changed {x}")
                    .ToPropertyEx(this, model => model.Value, 0, scheduler: RxApp.MainThreadScheduler, deferSubscription: true)
                    .DisposeWith(_cleanup);

        }

        public string Title { get; }

        public extern int Value { [ObservableAsProperty] get; }

        /// <inheritdoc />
        public void Dispose()
        {
            _cleanup?.Dispose();
        }
    }


    //public abstract class DashboardObject<TEntity> 
    //        where TEntity : AggregateRoot
    //{
    //    public IEntityCache<TEntity> EntityCache { get; }

    //    public DashboardObject(IEntityCache<TEntity> entityCache)
    //    {
    //        EntityCache = entityCache;


    //    }
    //}
}