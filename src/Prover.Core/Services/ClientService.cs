using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Prover.Core.Models.Clients;
using Prover.Core.Storage;

namespace Prover.Core.Services
{
    public class ClientService
    {
        private readonly IClientStore _clientStore;

        public ClientService(IClientStore clientStore)
        {
            _clientStore = clientStore;
        }

        public async Task<List<Client>> GetAll()
        {
            return await _clientStore.Query()
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }
}