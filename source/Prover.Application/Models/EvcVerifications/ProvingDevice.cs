using System;
using System.Collections.Generic;
using Prover.Shared.Domain;

namespace Prover.Application.Models.EvcVerifications {

	public class ApparatusGroup : EntityBase {
		public string Description { get; set; }
	}

	public class Apparatus : EntityBase {

		public ICollection<Prover> PairedProverTables { get; set; }

		public string SerialNumber { get; set; }

		public string Description { get; set; }

		public ApparatusGroup Group { get; set; }

		public TimeSpan RecalibrationPeriod { get; set; }

		public ICollection<ApparatusCalibrationRecord> CalibrationRecords { get; set; }
	}

	public class ApparatusCalibrationRecord : EntityBase {

		public DateTimeOffset CalibrationDate { get; set; }

		public DateTimeOffset ExpiryDate { get; set; }

		public string Notes { get; set; }

		public ICollection<string> CalibrationDocumentNames { get; set; }
	}

	public class EvcVerificationProvingDevices {
		public EvcVerificationTest VerificationTest { get; set; }

		public ICollection<Apparatus> Apparatuses { get; set; }
	}
}