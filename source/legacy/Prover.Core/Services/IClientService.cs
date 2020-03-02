using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prover.Core.Models.Clients;

namespace Prover.Core.Services
{
    public interface IClientService
    {
        Task ArchiveClient(Client client);
        Task<bool> DeleteCsvTemplate(ClientCsvTemplate template);
        IEnumerable<Client> GetActiveClients();
        List<Client> GetAllClients();
        Task<Client> GetById(Guid id);
        Task Save(Client client);
    }
}