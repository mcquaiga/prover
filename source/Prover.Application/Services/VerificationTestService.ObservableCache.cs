using System;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using DynamicData;
using Microsoft.Extensions.Logging;
using Prover.Application.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Shared.Extensions;

namespace Prover.Application.Services
{
    public partial class VerificationTestService : IEntityDataCache<EvcVerificationTest>, ICacheManager
    {
        private readonly Expression<Func<EvcVerificationTest, bool>> _defaultPredicate = test => test.TestDateTime.IsLessThanTimeAgo(TimeSpan.FromDays(30));
        private readonly ISourceCache<EvcVerificationTest, Guid> _cacheUpdates = new SourceCache<EvcVerificationTest, Guid>(k => k.Id);
        private CompositeDisposable _cleanup;
        private IObservableList<EvcVerificationTest> _data;

        public IObservableCache<EvcVerificationTest, Guid> Updates { get; private set; }

        public IObservableList<EvcVerificationTest> Data(Func<EvcVerificationTest, bool> filter = null) => _data.Connect(filter).AsObservableList();

        /// <inheritdoc />
        public IObservableCache<EvcVerificationTest, Guid> FetchTests() => Load();

        public IObservableCache<EvcVerificationTest, Guid> Load()
        {
            _ = LoadAsync();

            return Updates;
        }

        /// <inheritdoc />
        public Task LoadAsync()
        {
            if (_cleanup == null)
            {
                var loader = GetTests().Publish();

                _cacheUpdates.PopulateFrom(loader);

                Updates = _cacheUpdates.Connect()
                                       .Filter(v => v.TestDateTime.IsLessThanTimeAgo(TimeSpan.FromDays(30)))
                                       .AsObservableCache();
                
                _cleanup = new CompositeDisposable(
                        loader.Connect(), Updates, _data, _cacheUpdates);

                //LogChanges().DisposeWith(_cleanup);
            }

            return Task.CompletedTask;
        }

        public void Update()
        {
            _cacheUpdates.Refresh(
                    GetTests().ToListObservable());
        }

        private IObservable<EvcVerificationTest> GetTests(Expression<Func<EvcVerificationTest, bool>> predicate = null) => 
                _verificationRepository.Query(predicate).ToObservable();

        private void SetupCache()
        {
            Updates = _cacheUpdates.AsObservableCache();
            _data = Updates.Connect().RemoveKey().AsObservableList();
            LoadAsync();
        }

        private IDisposable LogChanges()
        {
            const string messageTemplate = "{0} {1} {2} ({4}). Verified = {3}";
            return Updates.Connect().Skip(1)
                      .WhereReasonsAre(ChangeReason.Add,ChangeReason.Update)
                      .Cast(test => string.Format(messageTemplate,
                              test.Id,
                              test.TestDateTime,
                              test.Device.DeviceType,
                              test.Device.CompositionShort(),
                              test.Verified))
                      .ForEachChange(change=>_logger.LogDebug(change.Current))
                      .Subscribe();

        }

        public void Dispose()
        {
            _cleanup?.Dispose();
        }
    }
}

//var updates = _cacheUpdates.AsObservableCache();

//updates.Connect()
//       .LogDebug(x => $"updates")
//       .Bind(out var items)
//       .Subscribe();
//Items = items;

//Changes = updates.AsObservableCache();

//_cachedData = _cacheUpdates.Connect().Publish();

//Data = _cachedData.AsObservableCache();
//List = Data.Connect().RemoveKey().AsObservableList();