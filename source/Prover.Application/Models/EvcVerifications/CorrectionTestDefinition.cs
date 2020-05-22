using System;
using System.Collections.Generic;

namespace Prover.Application.Models.EvcVerifications
{

	public static class VerificationDefaults
	{
		static VerificationDefaults()
		{
			VerificationOptions = new VerificationTestOptions()
			{
				CorrectionTestDefinitions = new List<CorrectionTestDefinition>(CorrectionDefinition),
				VolumeTestDefinitions = new List<VolumeTestDefinition> { VolumeDefinition }
			};
		}

		public static VerificationTestOptions VerificationOptions { get; }

		public static VolumeTestDefinition VolumeDefinition { get; } = new VolumeTestDefinition() { Level = 0, VolumeInputTargets = new List<VolumeInputTestSample>() };

		public static ICollection<CorrectionTestDefinition> CorrectionDefinition { get; } = new List<CorrectionTestDefinition>
		{
				new CorrectionTestDefinition
				{
						Level = 0,
						TemperatureGauge = 32,
						PressureGaugePercent = 80,
						IsVolumeTest = true
				},
				new CorrectionTestDefinition
				{
						Level = 1,
						TemperatureGauge = 60,
						PressureGaugePercent = 50,
						IsVolumeTest = false
				},
				new CorrectionTestDefinition
				{
						Level = 2,
						TemperatureGauge = 90,
						PressureGaugePercent = 20,
						IsVolumeTest = false
				}
		};

		public static ProvingApparatus ProvingApparatus = new ProvingApparatus();

		public class DefaultApparatus : ProvingApparatus
		{
			public DefaultApparatus()
			{
				Id = Guid.Parse("3a158629-3761-4fd3-9717-ed71afc6db9f");
				DisplayName = "EVC Proving Apparatus";
				ApparatusDescriptor = "EVC Proving Apparatus";
			}
		}
	}

	public class VerificationTestOptions
	{
		public ICollection<CorrectionTestDefinition> CorrectionTestDefinitions { get; set; }

		public ICollection<VolumeTestDefinition> VolumeTestDefinitions { get; set; }
	}

	public class VolumeTestDefinition
	{
		public ICollection<VolumeInputTestSample> VolumeInputTargets { get; set; }
		public int Level { get; set; }

	}

	public class CorrectionTestDefinition
	{
		#region Public Properties

		public bool IsVolumeTest { get; set; } = false;

		public int Level { get; set; }

		//public List<MechanicalUncorrectedTestLimit> MechanicalDriveTestLimits { get; set; } = new List<MechanicalUncorrectedTestLimit>();

		public decimal PressureGaugePercent { get; set; }

		public decimal TemperatureGauge { get; set; }

		#endregion


	}

	public class VolumeInputTestSample
	{
		#region Public Properties

		/// <summary>
		///     Gets or sets the CuFtValue
		/// </summary>
		public decimal CuFtValue { get; set; }

		/// <summary>
		///     Gets or sets the UncorrectedPulses
		/// </summary>
		public int UncorrectedPulseTarget { get; set; }

		#endregion
	}
}