using Devices.Core.Items.ItemGroups;
using Newtonsoft.Json;
using Prover.Calculations;
using Prover.Shared;

namespace Prover.Application.Models.EvcVerifications.Verifications.Volume
{
	public class UncorrectedVolumeTestRun : VerificationTestEntity<VolumeItems, VolumeItems>, IPulseOutputVerification
	{
		protected UncorrectedVolumeTestRun(VolumeInputType driveInputType) => DriveInputType = driveInputType;

		[JsonConstructor]
		public UncorrectedVolumeTestRun
		(VolumeInputType driveInputType, VolumeItems startValues, VolumeItems endValues, decimal expectedValue, decimal actualValue, decimal percentError, bool verified,
				decimal appliedInput, decimal driveRate) : base(startValues, endValues, expectedValue, actualValue, percentError, verified)
		{
			DriveInputType = driveInputType;
			AppliedInput = appliedInput;
			DriveRate = driveRate;
		}

		public decimal DriveRate { get; protected set; }

		//public IVolumeInputType DriveInputType { get; } = VolumeInputBuilderFactory.GetBuilder().StartValues.VolumeInputType
		public VolumeInputType DriveInputType { get; protected set; }

		public decimal AppliedInput { get; set; }

		public PulseOutputVerification PulseOutputTest { get; set; }

		public void Calculate()
		{
			ActualValue = VolumeCalculator.TotalVolume(StartValues.UncorrectedReading, EndValues.UncorrectedReading, StartValues.UncorrectedMultiplier);
			ExpectedValue = GetInputVolume();
		}

		protected virtual decimal GetInputVolume() => VolumeCalculator.TrueUncorrected(DriveRate, AppliedInput);
	}

	public class RotaryUncorrectedVolumeTestRun : UncorrectedVolumeTestRun
	{
		//[JsonConstructor]

		/// <inheritdoc />
		//public RotaryUncorrectedVolumeTestRun(RotaryMeterItems rotaryItems, VolumeItems startValues, VolumeItems endValues, decimal appliedInput)
		//		: this(startValues, endValues, appliedInput,
		//		rotaryItems.MeterType?.MeterDisplacement ?? (rotaryItems.MeterDisplacement != 0 ? rotaryItems.MeterDisplacement : -1))
		//{
		//}

		//[JsonConstructor]
		//public RotaryUncorrectedVolumeTestRun(VolumeItems startValues, VolumeItems endValues, decimal appliedInput, decimal driveRate)

		//[JsonConstructor]
		//public RotaryUncorrectedVolumeTestRun(VolumeItems startValues, VolumeItems endValues, decimal appliedInput, decimal driveRate)
		//		: base(VolumeInputType.Rotary, startValues, endValues, appliedInput)
		//{
		//	DriveRate = driveRate;
		//	Calculate();
		//}
		/// <inheritdoc />
		protected RotaryUncorrectedVolumeTestRun() : base(VolumeInputType.Rotary)
		{
		}

		[JsonConstructor]
		public RotaryUncorrectedVolumeTestRun
				(VolumeItems startValues, VolumeItems endValues, decimal expectedValue, decimal actualValue, decimal percentError, bool verified, decimal appliedInput, decimal driveRate) : base(
		VolumeInputType.Rotary, startValues, endValues, expectedValue, actualValue, percentError, verified, appliedInput, driveRate)
		{
		}


	}

	public class MechanicalUncorrectedVolumeTestRun : UncorrectedVolumeTestRun
	{
		protected MechanicalUncorrectedVolumeTestRun() : base(VolumeInputType.Mechanical)
		{
		}

		/// <inheritdoc />
		//public MechanicalUncorrectedVolumeTestRun(VolumeItems startValues, VolumeItems endValues, decimal appliedInput)
		//		: base(VolumeInputType.Mechanical, startValues, endValues, appliedInput)
		//{
		//	Calculate();
		//}
		/// <inheritdoc />
		/// <inheritdoc />
		[JsonConstructor]
		public MechanicalUncorrectedVolumeTestRun
				(VolumeItems startValues, VolumeItems endValues, decimal expectedValue, decimal actualValue, decimal percentError, bool verified, decimal appliedInput, decimal driveRate) : base(
		VolumeInputType.Mechanical, startValues, endValues, expectedValue, actualValue, percentError, verified, appliedInput, driveRate)
		{
		}
	}
}