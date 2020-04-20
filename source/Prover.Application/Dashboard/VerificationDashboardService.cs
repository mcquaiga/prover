using System;
using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using ReactiveUI;

namespace Prover.Application.Dashboard
{
    public class VerificationDashboardService : ReactiveObject, IDisposable
    {
        private readonly CompositeDisposable _cleanup = new CompositeDisposable();
        private readonly IEntityDataCache<EvcVerificationTest> _verificationCache;
        private readonly IObservable<IChangeSet<EvcVerificationTest, Guid>> _cache;

        private ReadOnlyObservableCollection<EvcVerificationTest> _verified;
        private ReadOnlyObservableCollection<EvcVerificationTest> _today;

        public VerificationDashboardService(IEntityDataCache<EvcVerificationTest> verificationCache)
        {
            _cache = verificationCache.Updates.Connect();

            SetVerifiedTests();

            _cache.Filter(v => v.TestDateTime.IsToday())
                  .ObserveOn(RxApp.MainThreadScheduler)
                  .Bind(out _today)
                  .Subscribe()
                  .DisposeWith(_cleanup);
        }
        
        public ReadOnlyObservableCollection<EvcVerificationTest> Verified => _verified;
        public ReadOnlyObservableCollection<EvcVerificationTest> Today => _today;

        private void SetVerifiedTests(DateTime? fromDateTime = null, DateTime? toDateTime = null)
        {
            _cache.Filter(v => v.Verified)
                  .ObserveOn(RxApp.MainThreadScheduler)
                  .Bind(out _verified)
                  .Subscribe()
                  .DisposeWith(_cleanup);

            //&& (!fromDateTime.HasValue  ||
            //(fromDateTime.HasValue && toDateTime.HasValue && v.TestDateTime.Between(fromDateTime.Value, toDateTime.Value)))
        }

        

        /// <inheritdoc />
        public void Dispose()
        {
            _cleanup?.Dispose();
        }
    }
}