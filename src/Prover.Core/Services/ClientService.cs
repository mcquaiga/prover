using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Prover.Core.Models.Clients;
using Prover.Core.Shared.Data;
using Prover.Core.Storage;

namespace Prover.Core.Services
{
    public class ClientService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IProverStore<Client> _clientStore;
        private readonly IProverStore<ClientCsvTemplate> _csvTemplateStore;

        public ClientService(IDbContextScopeFactory dbContextScopeFactory, IProverStore<Client> clientStore, IProverStore<ClientCsvTemplate> csvTemplateStore)
        {
            _dbContextScopeFactory = dbContextScopeFactory;
            _clientStore = clientStore;
            _csvTemplateStore = csvTemplateStore;
        }

        public async Task<List<Client>> GetAllClients()
        {
            _dbContextScopeFactory.CreateReadOnly();
         
            var clients = _clientStore.GetAll();
            return await clients
                .OrderBy(c => c.Name)
                .ToListAsync();
           
        }

        public IEnumerable<Client> GetActiveClients()
        {
            _dbContextScopeFactory.CreateReadOnly();
            
                return _clientStore
                    .Query(x => x.ArchivedDateTime == null);
            
        }

        public async Task ArchiveClient(Client client)
        {
            client.ArchivedDateTime = DateTime.UtcNow;
            await Save(client);
        }

        public async Task<bool> DeleteCsvTemplate(ClientCsvTemplate template)
        {
            using (var db = _dbContextScopeFactory.Create())
            {
                await _csvTemplateStore.Delete(template);
                return await db.SaveChangesAsync() > 0;
            }            
        }

        public async Task Save(Client client)
        {
            using (var db = _dbContextScopeFactory.Create())
            {
                await _clientStore.Upsert(client);
                await db.SaveChangesAsync();
            }
        }
    }
}