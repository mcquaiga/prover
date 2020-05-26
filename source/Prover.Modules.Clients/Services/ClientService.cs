using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prover.Application.Specifications;
using Prover.Modules.Clients.Core;
using Prover.Modules.Clients.Core.Interfaces;
using Prover.Shared.Storage.Interfaces;

namespace Prover.Modules.Clients.Core.Services {
	public class ClientService {
		private readonly IAsyncRepository<Client> _repository;

		public ClientService(IAsyncRepository<Client> clientRepository, IClientItemsService itemsService, IClientCsvTemplateService csvTemplateService) {
			_repository = clientRepository;
		}

		public async Task<IEnumerable<Client>> GetAllClients() {
			var clients = await _repository.ListAsync();
			return clients;
		}

		public Task<Client> GetClient(Guid id) {
			return _repository.GetAsync(id);
		}

		public async Task<IEnumerable<Client>> GetActiveClients() {
			return await _repository.QueryAsync(QuerySpec.Where<Client>(c => !c.Archived.HasValue));
		}

		public Task<Client> Archive(Client client) {
			client.Archived = DateTime.Now;
			return _repository.UpsertAsync(client);
		}

		public async Task<Client> CreateClient(string name, string registrationId, Address address) {

			var client = new Client() {
				Name = name,
				RegistrationId = registrationId,
				Address = address
			};

			var clientItems = new ClientValidationRules(client);
			var csvTemplate = new ClientCsvTemplate(client);

			return _repository.UpsertAsync(client);
		}
		//public async Task<bool> DeleteCsvTemplate(ClientCsvTemplate template) {
		//	//await _csvTemplateStore.Delete(template);
		//	//return true;
		//}

		//public async Task Save(Client client) {
		//	//await _clientStore.Upsert(client);
		//}
	}

	public interface IClientCsvTemplateService {
	}

	public interface IClientItemsService {
	}
}