using Devices.Core.Repository;
using LiteDB;
using Prover.Domain.EvcVerifications;

namespace Prover.Infrastructure.KeyValueStore
{
    public class VerificationsLiteDbRepository : LiteDbAsyncRepository<EvcVerificationTest>
    {
        private readonly IDeviceRepository _deviceRepository;

        public VerificationsLiteDbRepository(ILiteDatabase context, IDeviceRepository deviceRepository) : base(context)
        {
            
            _deviceRepository = deviceRepository;
        }

        #region Nested type: Device

       

        #endregion
    }
}