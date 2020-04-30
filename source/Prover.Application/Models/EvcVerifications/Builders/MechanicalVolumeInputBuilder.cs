using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.DriveTypes;
using Prover.Application.Models.EvcVerifications.Verifications.Volume;
using Prover.Calculations;
using Prover.Shared;
using Prover.Shared.Extensions;
using System.Collections.Generic;

namespace Prover.Application.Models.EvcVerifications.Builders
{
	internal class MechanicalVolumeInputBuilder : VolumeInputTestBuilder
	{
		private EnergyItems _endEnergyItems;
		private EnergyItems _startEnergyItems;

		public MechanicalVolumeInputBuilder(DeviceInstance device) : base(device)
		{
		}

		/// <inheritdoc />
		//public override IVolumeInputType BuildVolumeType() => new MechanicalVolumeInputType(Device.ItemGroup<VolumeItems>());

		/// <inheritdoc />
		protected override UncorrectedVolumeTestRun GetDriveSpecificUncorrectedTest()
		{
			var expected = VolumeCalculator.TrueUncorrected(Device.Items.Volume.DriveRate, AppliedInput);
			var actual = VolumeCalculator.TotalVolume(StartItems.UncorrectedReading, EndItems.UncorrectedReading, StartItems.UncorrectedMultiplier);
			var error = Calculators.PercentDeviation(expected, actual);

			return new UncorrectedVolumeTestRun(VolumeInputType.Mechanical,
						StartItems,
						EndItems,
						expectedValue: expected,
						actualValue: actual,
						percentError: error,
						verified: error.IsBetween(Tolerances.UNCOR_ERROR_THRESHOLD),
						appliedInput: AppliedInput,
			driveRate: Device.Items.Volume.DriveRate);
		}

		/// <inheritdoc />
		public override void SetItemValues(ICollection<ItemValue> startValues, ICollection<ItemValue> endValues, int? appliedInput = null, int? corPulses = null, int? uncorPulses = null)
		{
			base.SetItemValues(startValues, endValues, appliedInput, corPulses, uncorPulses);

			_startEnergyItems = Device.CreateItemGroup<EnergyItems>(startValues);
			_endEnergyItems = Device.CreateItemGroup<EnergyItems>(endValues);
		}



		/// <inheritdoc />
		protected override void SpecificDefaults()
		{
			var actual = EnergyCalculator.TotalEnergy(_startEnergyItems.EnergyReading, _endEnergyItems.EnergyReading);
			var expected = EnergyCalculator.Calculated(_startEnergyItems.EnergyUnitType, Corrected.ExpectedValue, _endEnergyItems.EnergyGasValue);
			var error = Calculators.PercentDeviation(expected, actual);

			var energyTest = new EnergyTest(_startEnergyItems, _endEnergyItems, expected, actual, error, error.IsBetween(Tolerances.ENERGY_PASS_TOLERANCE));
			Tests.Add(energyTest);
		}
	}
}