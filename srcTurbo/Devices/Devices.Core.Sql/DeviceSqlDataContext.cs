using System;
using System.Collections.Generic;
using System.Text;
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
