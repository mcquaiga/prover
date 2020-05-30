using System;
using System.Collections.Generic;
using System.Text;
using Prover.Modules.Certificates.Models;
using Prover.Shared.Storage.Interfaces;

namespace Prover.Modules.Certificates.Core.Services {
	public class CertificateService {
		private readonly IAsyncRepository<Certificate> _repository;

		public CertificateService(IAsyncRepository<Certificate> repository) {
			_repository = repository;
		}
	}
}
