using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prover.Application.Specifications;
using Prover.Modules.Clients.Core;
using Prover.Modules.Clients.Core.Interfaces;
using Prover.Shared.Storage.Interfaces;

namespace Prover.Modules.Clients.Core.Services {

	public class RegionService {
		private readonly IAsyncRepository<Region> _repository;

		public RegionService(IAsyncRepository<Region> repository, IDeviceValidationRulesService validationService) {
			_repository = repository;
		}


	}

	public class OwnerService {
		private readonly IAsyncRepository<Owner> _repository;

		public OwnerService(IAsyncRepository<Owner> clientRepository,
				IDeviceValidationRulesService validationService,
				ICsvTemplateService csvTemplateService
			) {
			_repository = clientRepository;
		}

		public async Task<IEnumerable<Owner>> GetAllOwners() {
			var Owners = await _repository.ListAsync();
			return Owners;
		}

		public Task<Owner> GetOwner(Guid id) {
			return _repository.GetAsync(id);
		}

		public async Task<IEnumerable<Owner>> GetActiveOwner() {
			return await _repository.QueryAsync(QuerySpec.Where<Owner>(c => !c.Archived.HasValue));
		}

		public Task<Owner> Archive(Owner owner) {
			owner.Archived = DateTime.Now;
			return _repository.UpsertAsync(owner);
		}

		public async Task<Owner> CreateOwner(string name, string registrationId, Address address) {

			var client = new Owner() {
				Name = name,
				RegistrationId = registrationId,
				Address = address
			};

			var clientItems = new DeviceValidationRules();
			var csvTemplate = new CsvTemplate();

			return await _repository.UpsertAsync(client);
		}
		//public async Task<bool> DeleteCsvTemplate(ClientCsvTemplate template) {
		//	//await _csvTemplateStore.Delete(template);
		//	//return true;
		//}

		//public async Task Save(Client client) {
		//	//await _clientStore.Upsert(client);
		//}
	}

	public interface ICsvTemplateService {
	}

	public interface IDeviceValidationRulesService {
	}
}