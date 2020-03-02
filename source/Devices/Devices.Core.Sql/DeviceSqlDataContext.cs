using Microsoft.EntityFrameworkCore;

namespace Devices.Core.Database
{
    public class DeviceSqlDataContext : DbContext
    {
        public DeviceSqlDataContext(DbContextOptions<DeviceSqlDataContext> options) : base(options)
        {

        }
    }
}
