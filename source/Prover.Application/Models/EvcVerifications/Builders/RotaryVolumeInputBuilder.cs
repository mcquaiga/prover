using Devices.Core.Interfaces;
using Devices.Core.Items.DriveTypes;
using Prover.Application.Models.EvcVerifications.Verifications.Volume;
using Prover.Application.Models.EvcVerifications.Verifications.Volume.InputTypes.Rotary;
using Prover.Calculations;
using Prover.Shared.Extensions;

namespace Prover.Application.Models.EvcVerifications.Builders
{
    internal class RotaryVolumeInputBuilder : VolumeInputTestBuilder
    {

        //private readonly RotaryVolumeInputType _inputType;


        public RotaryVolumeInputBuilder(DeviceInstance device) : base(device)
        {
            // _inputType = new RotaryVolumeInputType(Device.ItemGroup<VolumeItems>(), );
        }

        /// <inheritdoc />
        protected override UncorrectedVolumeTestRun GetDriveSpecificUncorrectedTest()
        {
            var meterItems = Device.ItemGroup<RotaryMeterItems>();

            var expected = VolumeCalculator.TrueUncorrected(meterItems.MeterType.MeterDisplacement ?? meterItems.MeterDisplacement, AppliedInput);
            var actual = VolumeCalculator.TotalVolume(StartItems.UncorrectedReading, EndItems.UncorrectedReading, StartItems.UncorrectedMultiplier);
            var error = Calculators.PercentDeviation(expected, actual);

            return new RotaryUncorrectedVolumeTestRun(StartItems, EndItems, expectedValue: expected, actualValue: actual, percentError: error,
            verified: error.IsBetween(Tolerances.UNCOR_ERROR_THRESHOLD), appliedInput: AppliedInput, driveRate: meterItems.MeterType.MeterDisplacement ?? meterItems.MeterDisplacement);
        }

        /// <inheritdoc />
        protected override void SpecificDefaults()
        {
            var rotaryTest = new RotaryMeterTest(Device.ItemGroup<RotaryMeterItems>());
            Tests.Add(rotaryTest);

        }
    }
}