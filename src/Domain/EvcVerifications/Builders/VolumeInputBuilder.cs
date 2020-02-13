using System;
using Core.GasCalculations;
using Devices.Core.Interfaces;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;
using Domain.EvcVerifications.Verifications.CorrectionFactors;
using Domain.EvcVerifications.Verifications.Volume.InputTypes;
using Domain.EvcVerifications.Verifications.Volume.InputTypes.Rotary;

namespace Domain.EvcVerifications.Builders
{
    internal abstract class VolumeInputBuilder
    {
        #region Public Methods

        public virtual VolumeInputBuilder BuildVerificationTests(EvcVerificationTest evcVerification)
        {
            return this;
        }

        public virtual VolumeInputBuilder BuildVolumeTestPointTests(IVolumeInputType inputType, DeviceInstance device,
            VerificationTestPoint testPoint)
        {
            var totalCorrection = Calculators.TotalCorrectionFactor(
                testPoint.GetTest<TemperatureCorrectionTest>()
                    ?.ActualValue,
                testPoint.GetTest<PressureCorrectionTest>()
                    ?.ActualValue,
                testPoint.GetTest<SuperCorrectionTest>()
                    ?.ActualValue);


            var uncorVolume = inputType.UnCorrectedInputVolume(testPoint.AppliedInput ?? 0);

            var startVolumeItems = device.ItemGroup<VolumeItems>();
            var endVolumeItems = device.ItemGroup<VolumeItems>();

            //testPoint.AddTest(
            //    VolumeTestRunFactory.Factory.Create(startVolumeItems, endVolumeItems, totalCorrection,
            //        uncorVolume)
            //);

            //testPoint.AddTest(
            //    VolumeTestRunFactory.Factory.Create(startVolumeItems, endVolumeItems, uncorVolume)
            //);

            return this;
        }

        public abstract VolumeInputBuilder BuildVolumeInputType(DeviceInstance device, EvcVerificationTest evcVerification);

        #endregion
    }

     internal class RotaryVolumeInputBuilder : VolumeInputBuilder
    {
        #region Public Methods

        public override VolumeInputBuilder BuildVerificationTests(EvcVerificationTest evcVerification)
        {
            base.BuildVerificationTests(evcVerification);

            evcVerification.AddTest(
                new RotaryMeterTest(evcVerification.Device.ItemGroup<RotaryMeterItems>())
            );

            return this;
        }

        public override VolumeInputBuilder BuildVolumeInputType(DeviceInstance device, EvcVerificationTest evcVerification)
        {
            evcVerification.DriveType = new RotaryVolumeInputType(device.ItemGroup<VolumeItems>(),
                device.ItemGroup<RotaryMeterItems>());

            return this;
        }

        #endregion
    }

    internal class MechanicalVolumeInputBuilder : VolumeInputBuilder
    {
        #region Public Methods

        public override VolumeInputBuilder BuildVolumeInputType(DeviceInstance device, EvcVerificationTest evcVerification)
        {
            evcVerification.DriveType = new MechanicalVolumeInputType(device.ItemGroup<VolumeItems>());

            return this;
        }

        public override VolumeInputBuilder BuildVolumeTestPointTests(IVolumeInputType inputType, DeviceInstance device,
            VerificationTestPoint testPoint)
        {
            base.BuildVolumeTestPointTests(inputType, device, testPoint);

            var startValues = device.ItemGroup<EnergyItems>();
            var endValues = device.ItemGroup<EnergyItems>();

            //testPoint.AddTest(
            //    EnergyTest.Create(
            //        startValues,
            //        endValues,
            //        testPoint.GetTest<CorrectedVolumeTestRun>()?.ActualValue)
            //);

            return this;
        }

        #endregion
    }

    internal class PulseInputVolumeBuilder : VolumeInputBuilder
    {
        #region Public Methods

        public override VolumeInputBuilder BuildVerificationTests(EvcVerificationTest evcVerification)
        {
            throw new NotImplementedException();
        }

        public override VolumeInputBuilder BuildVolumeInputType(DeviceInstance device, EvcVerificationTest evcVerification)
        {
            evcVerification.DriveType = new PulseInputSensor(device);

            return this;
        }

        public override VolumeInputBuilder BuildVolumeTestPointTests(IVolumeInputType inputType, DeviceInstance device,
            VerificationTestPoint testPoint)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}