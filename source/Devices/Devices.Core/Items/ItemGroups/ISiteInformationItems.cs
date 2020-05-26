using System;
using Prover.Shared;

namespace Devices.Core.Items.ItemGroups {
	public class SiteInformationItems : ItemGroup {

		public virtual CompositionType CompositionType { get; set; }

		public virtual string FirmwareVersion { get; set; }
		public virtual string SerialNumber { get; set; }
		public virtual string SiteId1 { get; set; }
		public virtual string SiteId2 { get; set; }

		public virtual DateTime Date { get; set; }
		public virtual DateTime Time { get; set; }

		public virtual CorrectionFactorType LivePressureFactor { get; set; }
		public virtual CorrectionFactorType LiveSuperFactor { get; set; }
		public virtual CorrectionFactorType LiveTemperatureFactor { get; set; }
	}
}