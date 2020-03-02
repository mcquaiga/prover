using Devices.Core.Interfaces;
using LiteDB;

namespace Prover.Infrastructure.KeyValueStore
{

    public class DeviceTypeLiteDbRepository : LiteDbRepository<DeviceType>
    {
        public DeviceTypeLiteDbRepository(ILiteDatabase context) : base(context)
        {
           
        }

    }
}