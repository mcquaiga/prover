using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Prover.Core.Models.Clients;
using Prover.Core.Shared.Data;

namespace Prover.Core.Storage
{
    public class ClientStore : ProverStore<Client>
    {
        public ClientStore(ProverContext dbContext) : base(dbContext)
        {
        }
    }
}