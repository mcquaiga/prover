using Devices.Core.Interfaces;
using LiteDB;

namespace Prover.Storage.LiteDb
{

    public class DeviceTypeLiteDbRepository : LiteDbRepository<DeviceType>
    {
        public DeviceTypeLiteDbRepository(ILiteDatabase context) : base(context)
        {
           
        }

    }
}