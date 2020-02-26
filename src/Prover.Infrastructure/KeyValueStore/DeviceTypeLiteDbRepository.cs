using Devices.Core.Interfaces;
using LiteDB;
using Prover.Shared.Interfaces;

namespace Prover.Infrastructure.KeyValueStore
{

    public class DeviceTypeLiteDbRepository : LiteDbRepository<DeviceType>
    {
        public DeviceTypeLiteDbRepository(ILiteDatabase context) : base(context)
        {
           
        }

    }
}