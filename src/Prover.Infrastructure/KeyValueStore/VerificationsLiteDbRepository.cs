using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Repository;
using LiteDB;
using Prover.Domain.EvcVerifications;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

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