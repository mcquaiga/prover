using System;
using System.Linq.Expressions;
using System.Reactive.Linq;
using Devices.Core.Repository;
using LiteDB;
using Prover.Application.Models.EvcVerifications;
using Prover.Shared.Interfaces;

namespace Prover.Infrastructure.KeyValueStore
{
    public class VerificationsLiteDbRepository : LiteDbAsyncRepository<EvcVerificationTest>, IObservableRepository<EvcVerificationTest>
    {
        private readonly IDeviceRepository _deviceRepository;

        public VerificationsLiteDbRepository(ILiteDatabase context, IDeviceRepository deviceRepository) : base(context)
        {
            _deviceRepository = deviceRepository;
        }

        public IQbservable<EvcVerificationTest> QueryObservable(Expression<Func<EvcVerificationTest, bool>> predicate = null)
        {
            var results = Query(predicate);

            return results.ToObservable().AsQbservable();
        }
    }
}