using Devices.Core.Interfaces;
using Prover.Shared;
using Prover.Shared.Domain;

namespace Prover.Modules.Clients.Core {
	public class CsvTemplate : EntityBase {
		public CsvTemplate() {
		}

		//public Client Client { get; set; }

		public VerificationType VerificationType { get; set; }

		public IDevice DeviceType { get; set; }

		public VolumeInputType DriveType { get; set; }

		public string Template { get; set; }
	}
}