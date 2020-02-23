using System;
using System.Collections.Generic;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Shared;

namespace Domain.EvcVerifications.Builders
{
    public class EvcVerificationBuilder
    {
        private readonly Dictionary<VolumeInputType, Func<IVolumeInputBuilder>> _volumeInputTypeBuilders =
            new Dictionary<VolumeInputType, Func<IVolumeInputBuilder>>
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

            VolumeBuilder(_device.ItemGroup<VolumeItems>().VolumeInputType)
                .BuildVolumeInputType(_device);
                //.CreateVerificationTests(_instance);
        }

        public EvcVerificationTest GetEvcVerification()
        {
            var instance = _instance;
            _instance = null;
            return instance;
        }

        public void SetTestDateTime()
        {
            _instance.TestDateTime = DateTime.Now;
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

        private IVolumeInputBuilder VolumeBuilder(VolumeInputType volumeInputType)
        {
            return _volumeInputTypeBuilders[volumeInputType].Invoke();
        }

        #endregion
    }

    public class VerificationTestPointBuilder
    {
        private readonly Func<VerificationTestPoint, VerificationTestPoint> _callback;

        private readonly EvcVerificationTest _evcVerification;
        private readonly IVolumeInputBuilder _volumeBuilder;


        private VerificationTestPoint _testPoint;

        internal VerificationTestPointBuilder(EvcVerificationTest evcVerification, IVolumeInputBuilder volumeBuilder, Func<VerificationTestPoint, VerificationTestPoint> callback)
        {
            _evcVerification = evcVerification;
            _volumeBuilder = volumeBuilder;
            _callback = callback;
            
        }

        public VerificationTestPointBuilder CreateNew(int level)
        {
            _testPoint = new VerificationTestPoint(level);

            return this;
        }

        #region Public Methods

        public VerificationTestPointBuilder BuildPressureTest(PressureItems pressureItems, decimal gauge, decimal? atmGauge)
        {
            //if (pressureItems != null)
            //{
            //    var p = pressureItems;
            //    var calc = new PressureCalculator(p.UnitType, p.TransducerType, p.Base, gauge, p.AtmosphericPressure);

            //    _testPoint.AddTest(
            //        new PressureCorrectionTest(pressureItems, gauge, atmGauge ?? pressureItems.AtmosphericPressure)
            //    );
               
            //}

            return this;
        }

        public VerificationTestPointBuilder BuildSuperFactorTest(SuperFactorItems superItems)
        {
            //var temp = _testPoint.GetVerificationTest<TemperatureCorrectionTest>();
            //var pressure =  _testPoint.GetVerificationTest<PressureCorrectionTest>();

            //if (temp != null && pressure != null)
            //{
            //    var si = superItems;

            //    var calc = new SuperFactorCalculator(si.Co2, si.N2, si.SpecGr, temp.Gauge, pressure.Gauge);

            //    _testPoint.AddTest(
            //        new SuperCorrectionTest(superItems, temp, pressure)
            //    );
            //}

            return this;
        }

        public VerificationTestPointBuilder BuildTemperatureTest(TemperatureItems tempItems, decimal gaugeTemp)
        {
            //if (tempItems != null)
            //{
            //    var tempTest = CorrectionTestFactory.CreateTemperatureTest(tempItems, gaugeTemp);

            //    //ICorrectionCalculator Calc(TemperatureCorrectionTest temp) => new TemperatureCalculator(temp.Values.Units, temp.Values.Base, temp.Gauge);

            //    //tempTest.WithCalculator((Func<VerificationTestEntity<TemperatureItems>, ICorrectionCalculator>)Calc);

            //    _testPoint.AddTest(
            //        tempTest
            //    );
            //}
            
            return this;
        }

        public VerificationTestPointBuilder BuildVolumeTest(VolumeItems startVolumeItems, VolumeItems endVolumeItems,
            decimal appliedInput)
        {
            _testPoint.AppliedInput = appliedInput;

            //_volumeBuilder.CreateTestPointTests(_evcVerification.DriveType, _evcVerification.Device, _testPoint);

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

   
}