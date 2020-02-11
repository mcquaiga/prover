using System;
using System.Collections.Generic;
using Core.GasCalculations;
using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.DriveTypes;
using Devices.Core.Items.ItemGroups;
using Domain.EvcVerifications.CorrectionTests;
using Domain.EvcVerifications.DriveTypes;

namespace Domain.EvcVerifications.Builders
{
    public class EvcVerificationBuilder
    {
        private readonly Dictionary<VolumeInputType, Func<VolumeInputBuilder>> _volumeInputTypeBuilders =
            new Dictionary<VolumeInputType, Func<VolumeInputBuilder>>
            {
                {VolumeInputType.Rotary, () => new RotaryVolumeInputBuilder()},
                {VolumeInputType.Mechanical, () => new MechanicalVolumeInputBuilder()},
                {VolumeInputType.PulseInput, () => new PulseInputVolumeBuilder()}
            };

        private DeviceInstance _device;
        private EvcVerificationTest _instance;

        public EvcVerificationBuilder(DeviceInstance device)
        {
            CreateNew(device);
        }

        #region Public Methods

        public void CreateNew(DeviceInstance device)
        {
            _device = device;
            _instance = new EvcVerificationTest(device);

            VolumeBuilder(_device.ItemGroup<IVolumeItems>().VolumeInputType)
                .BuildVolumeInputType(_device, _instance)
                .BuildVerificationTests(_instance);
        }

        public EvcVerificationTest GetEvcVerification()
        {
            var instance = _instance;
            _instance = null;
            return instance;
        }

        public void SetTestDateTime()
        {
            _instance.TestDateTime = DateTimeOffset.UtcNow;
        }

        public VerificationTestPointBuilder TestPointFactory()
        {
            return new VerificationTestPointBuilder(_instance, VolumeBuilder(_instance.DriveType.InputType),
                CompleteBuildingTestPoint);
        }

        #endregion

        #region Private

        private VerificationTestPoint CompleteBuildingTestPoint(VerificationTestPoint testPoint)
        {
            _instance.AddTest(testPoint);
            return testPoint;
        }

        private VolumeInputBuilder VolumeBuilder(VolumeInputType volumeInputType)
        {
            return _volumeInputTypeBuilders[volumeInputType].Invoke();
        }

        #endregion
    }

    public class VerificationTestPointBuilder
    {
        private readonly Func<VerificationTestPoint, VerificationTestPoint> _callback;

        private readonly EvcVerificationTest _evcVerification;
        private readonly VolumeInputBuilder _volumeBuilder;


        private VerificationTestPoint _testPoint;

        internal VerificationTestPointBuilder(EvcVerificationTest evcVerification, VolumeInputBuilder volumeBuilder, Func<VerificationTestPoint, VerificationTestPoint> callback)
        {
            _evcVerification = evcVerification;
            _volumeBuilder = volumeBuilder;
            _callback = callback;
            
        }

        public VerificationTestPointBuilder CreateNew(int level, IEnumerable<ItemValue> beforeValues,
            IEnumerable<ItemValue> afterValues)
        {
            _testPoint = new VerificationTestPoint(level)
            {
                BeforeValues = beforeValues,
                AfterValues = afterValues
            };

            return this;
        }

        #region Public Methods

        public VerificationTestPointBuilder BuildPressureTest(IPressureItems pressureItems, decimal gauge)
        {
            if (pressureItems != null)
            {
                var p = pressureItems;
                var calc = new PressureCalculator(p.UnitType, p.TransducerType, p.Base, gauge, p.AtmosphericPressure);

                _testPoint.AddTest(
                    CorrectionFactory.Create(CorrectionFactorTestType.Pressure, calc, p.Factor, gauge)
                );
               
            }

            return this;
        }

        public VerificationTestPointBuilder BuildSuperFactorTest(ISuperFactorItems superItems)
        {
            var temp = _testPoint.GetCorrectionTest<CorrectionTestWithGauge>(CorrectionFactorTestType.Temperature);
            var pressure =  _testPoint.GetCorrectionTest<CorrectionTestWithGauge>(CorrectionFactorTestType.Pressure);

            if (temp != null && pressure != null)
            {
                var si = superItems;

                var calc = new SuperFactorCalculator(si.Co2, si.N2, si.SpecGr, temp.Gauge, pressure.Gauge);

                _testPoint.AddTest(
                    CorrectionFactory.Create(CorrectionFactorTestType.Super, calc, si.Factor)
                );
            }

            return this;
        }

        public VerificationTestPointBuilder BuildTemperatureTest(ITemperatureItems tempItems, decimal gaugeTemp)
        {
            if (tempItems != null)
            {
                var calc = new TemperatureCalculator(tempItems.Units, tempItems.Base, gaugeTemp);

                _testPoint.AddTest(
                    CorrectionFactory.Create(CorrectionFactorTestType.Temperature, calc, tempItems.Factor, gaugeTemp)
                );
            }
            
            return this;
        }

        public VerificationTestPointBuilder BuildVolumeTest(IVolumeItems startVolumeItems, IVolumeItems endVolumeItems,
            decimal appliedInput)
        {
            _testPoint.AppliedInput = appliedInput;

            _volumeBuilder.BuildVolumeTestPointTests(_evcVerification.DriveType, _evcVerification.Device, _testPoint);

            return this;
        }


        public VerificationTestPoint Commit()
        {
            var tp = _testPoint;
            _testPoint = null;
            return _callback.Invoke(tp);
        }

        #endregion
    }

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
                testPoint.GetTest<CorrectionTest>(c => c.TestType == CorrectionFactorTestType.Temperature)
                    ?.ActualValue,
                testPoint.GetTest<CorrectionTest>(c => c.TestType == CorrectionFactorTestType.Pressure)
                    ?.ActualValue,
                testPoint.GetTest<CorrectionTest>(c => c.TestType == CorrectionFactorTestType.Super)
                    ?.ActualValue);


            var uncorVolume = inputType.UnCorrectedInputVolume(testPoint.AppliedInput ?? 0);

            var startVolumeItems = device.ItemGroup<IVolumeItems>(testPoint.BeforeValues);
            var endVolumeItems = device.ItemGroup<IVolumeItems>(testPoint.AfterValues);

            testPoint.AddTest(
                CorrectedVolumeTestRunFactory.Factory.Create(startVolumeItems, endVolumeItems, totalCorrection,
                    uncorVolume)
            );

            testPoint.AddTest(
                UncorrectedVolumeTestRun.Create(startVolumeItems, endVolumeItems, uncorVolume)
            );

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
                new RotaryMeterTest(evcVerification.Device.ItemGroup<IRotaryMeterItems>())
            );

            return this;
        }

        public override VolumeInputBuilder BuildVolumeInputType(DeviceInstance device, EvcVerificationTest evcVerification)
        {
            evcVerification.DriveType = new RotaryVolumeInputType(device.ItemGroup<IVolumeItems>(),
                device.ItemGroup<IRotaryMeterItems>());

            return this;
        }

        #endregion
    }

    internal class MechanicalVolumeInputBuilder : VolumeInputBuilder
    {
        #region Public Methods

        public override VolumeInputBuilder BuildVolumeInputType(DeviceInstance device, EvcVerificationTest evcVerification)
        {
            evcVerification.DriveType = new MechanicalVolumeInputType(device.ItemGroup<IVolumeItems>());

            return this;
        }

        public override VolumeInputBuilder BuildVolumeTestPointTests(IVolumeInputType inputType, DeviceInstance device,
            VerificationTestPoint testPoint)
        {
            base.BuildVolumeTestPointTests(inputType, device, testPoint);

            var startValues = device.ItemGroup<IEnergyItems>(testPoint.BeforeValues);
            var endValues = device.ItemGroup<IEnergyItems>(testPoint.AfterValues);

            testPoint.AddTest(
                EnergyTest.Create(
                    startValues,
                    endValues,
                    testPoint.GetTest<CorrectedVolumeTestRun>()?.ActualValue)
            );

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