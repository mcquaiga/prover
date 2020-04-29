using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Prover.Application.Models.EvcVerifications.Verifications;
using Prover.Application.Models.EvcVerifications.Verifications.Volume;
using Prover.Calculations;
using Prover.Shared;
using Prover.Shared.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Prover.Application.Models.EvcVerifications.Builders
{
	public abstract class VolumeInputTestBuilder
	{
		protected int AppliedInput;
		protected int CorPulses;
		protected VolumeItems EndItems;
		protected VolumeItems StartItems;
		protected int UncorPulses;

		protected UncorrectedVolumeTestRun Uncorrected;
		protected CorrectedVolumeTestRun Corrected;
		public VerificationTestPoint VerificationTestPoint { get; set; }

		protected VolumeInputTestBuilder(DeviceInstance device)
		{
			Device = device;
			StartItems = Device.CreateItemGroup<VolumeItems>();
			EndItems = Device.CreateItemGroup<VolumeItems>();
		}

		protected DeviceInstance Device { get; }
		protected List<VerificationEntity> Tests { get; } = new List<VerificationEntity>();


		public virtual VolumeInputTestBuilder AddCorrected(UncorrectedVolumeTestRun uncorrected, bool withPulseOutputs = true)
		{
			Corrected = new CorrectedVolumeTestRun(StartItems, EndItems, uncorrected.ExpectedValue,
			VerificationTestPoint.GetTemperature()?.ExpectedValue,
			VerificationTestPoint.GetPressure()?.ExpectedValue, VerificationTestPoint.GetSuper()?.ExpectedValue);

			if (withPulseOutputs)
				Corrected.PulseOutputTest = WithPulseOutput(PulseOutputType.CorVol, CorPulses, Corrected.ExpectedValue, Device.Items.Volume.CorrectedMultiplier);
			Tests.Add(Corrected);
			return this;
		}

		public VolumeInputTestBuilder AddDefaults(bool withPulseOutputs = true)
		{
			AddUncorrected(withPulseOutputs);
			AddCorrected(Uncorrected);

			SpecificDefaults();

			return this;
		}


		public VolumeInputTestBuilder AddUncorrected(bool withPulseOutputs = true)
		{
			//Uncorrected = new UncorrectedVolumeTestRun(BuildVolumeType(), _startItems, _endItems, _appliedInput);
			Uncorrected = GetDriveSpecificUncorrectedTest();

			if (withPulseOutputs)
				Uncorrected.PulseOutputTest = WithPulseOutput(PulseOutputType.UncVol, UncorPulses, Uncorrected.ExpectedValue, Device.Items.Volume.UncorrectedMultiplier);
			Tests.Add(Uncorrected);
			return this;
		}

		protected abstract UncorrectedVolumeTestRun GetDriveSpecificUncorrectedTest();

		public ICollection<VerificationEntity> Build() => Tests;

		public virtual void SetItemValues(ICollection<ItemValue> startValues, ICollection<ItemValue> endValues, int? appliedInput = null, int? corPulses = null, int? uncorPulses = null)
		{
			StartItems = Device.CreateItemGroup<VolumeItems>(startValues) ?? StartItems;
			EndItems = Device.CreateItemGroup<VolumeItems>(endValues) ?? EndItems;
			AppliedInput = appliedInput ?? 0;
			UncorPulses = uncorPulses ?? 0;
			CorPulses = corPulses ?? 0;
		}

		protected abstract void SpecificDefaults();



		protected virtual PulseOutputVerification WithPulseOutput(PulseOutputType pulseType, int pulseCount, decimal expectedVolume, decimal? multiplier)
		{
			var items = Device.ItemGroup<PulseOutputItems>().Channels.FirstOrDefault(c => c.ChannelType == pulseType);

			var expected = VolumeCalculator.PulseCount(expectedVolume, (items as IVolumeUnits)?.Units.Multiplier ?? multiplier);
			var error = Calculators.PercentDeviation(expected, pulseCount);

			return items != null ? new PulseOutputVerification(items, pulseCount, expected, error, error.IsBetween(Tolerances.PULSE_VARIANCE_THRESHOLD)) : default;
		}
	}
}