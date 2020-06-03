using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prover.Application.Models.EvcVerifications;
using Prover.Modules.Certificates.Models;
using Prover.Modules.Clients.Core;
using Prover.Shared.Interfaces;
using Prover.Shared.Storage.Interfaces;

namespace Prover.Modules.Certificates.Core.Services {
	public class CertificateService {
		private readonly IAsyncRepository<Certificate> _repository;

		public CertificateService(IAsyncRepository<Certificate> repository) {
			_repository = repository;
		}

		public void CreateCertificate(IEnumerable<EvcVerificationTest> verifications, IUser createdByUser, Owner owner) {

		}

		public void CreateCertificate(IEnumerable<EvcVerificationTest> verifications, IUser createdByUser, Region region) {
			var cert = new Certificate() {
				EvcVerifications = verifications.ToList(),
				CreatedBy = createdByUser.UserName,

			};
		}
	}
}
