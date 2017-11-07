using System.Data.Entity;
using System.Linq;
using Prover.Core.Models.Clients;
using Prover.Core.Shared.Data;

namespace Prover.Core.Storage
{
    public class ClientStore : EfProverStore<Client>
    {    
        public ClientStore(IAmbientDbContextLocator ambientDbContextLocator) : base(ambientDbContextLocator)
        {
        }

        protected override IQueryable<Client> Query()
        {
            return Context.Clients;
        }
    }
}