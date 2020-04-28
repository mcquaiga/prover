using Devices.Core.Repository;
using LiteDB;
using Prover.Application.Models.EvcVerifications;
using Prover.Shared.Storage.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;

namespace Prover.Storage.LiteDb
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
            return Observable.StartAsync(() => Query(predicate)).SelectMany(t => t).AsQbservable();


        }
    }
}