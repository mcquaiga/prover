using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Prover.Core.Models.Clients;

namespace Prover.Core.Storage
{
    public interface IClientStore : IProverStore<Client>
    {
        Task<List<Client>> GetAll();
        Task<bool> DeleteCsvTemplate(ClientCsvTemplate template);
    }

    public class ClientStore : IClientStore
    {
        private readonly ProverContext _context;

        public ClientStore(ProverContext context)
        {
            _context = context;
        }

        public IQueryable<Client> Query()
        {
            return _context.Clients
                .Include(x => x.Items)
                .AsQueryable();
        }

        public Client Get(Guid id)
        {
            return Query().FirstOrDefault(x => x.Id == id);
        }

        public async Task<List<Client>> GetAll()
        {
            return await Query()
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<bool> DeleteCsvTemplate(ClientCsvTemplate template)
        {
            _context.ClientCsvTemplates.Remove(template);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Client> UpsertAsync(Client entity)
        {
            if (Get(entity.Id) != null)
            {
                _context.Clients.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                _context.Clients.Add(entity);
            }

            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task Delete(Client entity)
        {
            entity.ArchivedDateTime = DateTime.UtcNow;
            await UpsertAsync(entity);
        }
    }
}