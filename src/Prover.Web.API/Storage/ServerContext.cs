using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Prover.Web.API.Storage
{
    public class ServerContext : DbContext
    {
        public ServerContext() : base(@"name=ConnectionString")
        {
            
        }
    }
}
