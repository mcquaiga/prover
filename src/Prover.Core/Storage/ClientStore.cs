using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Prover.Core.Models.Clients;

namespace Prover.Core.Storage
{
    public class ClientStore : ProverStore<Client>
    {
        public ClientStore(ProverContext dbContext) : base(dbContext)
        {
        }

        protected override IQueryable<Client> QueryCommand()
        {
            return Context.Clients
                .Include(x => x.Items);
        }
    }
}