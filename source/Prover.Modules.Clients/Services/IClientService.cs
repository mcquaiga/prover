using System;
using Prover.Modules.Clients.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prover.Modules.Clients.Core.Interfaces {
	public interface IClientService {
		Task ArchiveClient(Client client);
		Task<bool> DeleteCsvTemplate(ClientCsvTemplate template);
		IEnumerable<Client> GetActiveClients();
		List<Client> GetAllClients();
		Task<Client> GetById(Guid id);
		Task Save(Client client);
	}
}