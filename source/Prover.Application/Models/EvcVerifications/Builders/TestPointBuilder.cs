using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Prover.Application.Models.EvcVerifications.Verifications.CorrectionFactors;
using System;
using System.Collections.Generic;
using Prover.Application.Extensions;

namespace Prover.Application.Models.EvcVerifications.Builders
{

    public class TestPointBuilder
    {
        public readonly DeviceInstance Device;
        private readonly ICollection<ItemValue> _deviceValues;
        private readonly VerificationTestPoint _testPoint;
        private readonly VolumeInputTestBuilder _volumeBuilder;

        internal TestPointBuilder(DeviceInstance device, int testsCount, ICollection<ItemValue> deviceValues = null)
        {
            Device = device;
            _deviceValues = deviceValues.IsNotNullOrEmpty() ? deviceValues : Device.Values;
            _testPoint = new VerificationTestPoint(testsCount);

            _volumeBuilder = VolumeInputBuilderFactory.GetBuilder(device);
            _volumeBuilder.VerificationTestPoint = _testPoint;
        }

        public VerificationTestPoint Build() => _testPoint;

        public TestPointBuilder WithPressure(decimal gauge, decimal? atmGauge, PressureItems pressureItems = null)
        {
            if (Device.HasLivePressure())
            {
                pressureItems = pressureItems ?? Device.CreateItemGroup<PressureItems>(_deviceValues);
                _testPoint.AddTest(new PressureCorrectionTest(pressureItems, gauge, atmGauge ?? pressureItems.AtmosphericPressure));
            }

            return this;
        }



        public TestPointBuilder WithSuperFactor(SuperFactorItems items = null)
        {
            if (Device.HasLiveSuper())
            {
                var temp = _testPoint.GetVerificationTest<TemperatureCorrectionTest>();
                var pressure = _testPoint.GetVerificationTest<PressureCorrectionTest>();
                var si = items ?? Device.CreateItemGroup<SuperFactorItems>(_deviceValues);

                _testPoint.AddTest(new SuperCorrectionTest(si, temp, pressure));
            }

            return this;
        }

        public TestPointBuilder WithTemperature(decimal gaugeTemp, TemperatureItems temperatureItems = null)
        {
            if (Device.HasLiveTemperature())
            {
                temperatureItems = temperatureItems ?? Device.CreateItemGroup<TemperatureItems>(_deviceValues);
                _testPoint.AddTest(new TemperatureCorrectionTest(temperatureItems, gaugeTemp));
            }

            return this;
        }

        public TestPointBuilder WithVolume(ICollection<ItemValue> startValues = null, ICollection<ItemValue> endValues = null, Func<VolumeInputTestBuilder, VolumeInputTestBuilder> testDecorator = null,
                int appliedInput = 0, int corPulses = 0, int uncorPulses = 0)
        {
            _volumeBuilder.SetItemValues(startValues, endValues, appliedInput, corPulses, uncorPulses);

            var volumeTests = testDecorator?.Invoke(_volumeBuilder) ?? _volumeBuilder.AddDefaults();

            _testPoint.AddTests(volumeTests.Build());

            return this;
        }
    }
}

//public class TestPointBuilder
//{
//    private readonly DeviceInstance _device;
//    private static int _testCount;
//    private readonly List<VerificationTestPoint> _tests = new List<VerificationTestPoint>();
//    private readonly VolumeInputBuilder _volumeBuilder;
//    private VerificationTestPoint _current;

//    static TestPointBuilder()
//    {
//        _testCount = -1;
//    }

//    private TestPointBuilder(DeviceInstance device, VolumeInputBuilder volumeBuilder)
//    {
//        _device = device;
//        _volumeBuilder = volumeBuilder;

//    }

//    public static TestPointBuilder Create(DeviceInstance deviceInstance, VolumeInputBuilder volumeBuilder, CorrectionTestDefinition options = null)
//    {

//        return new TestPointBuilder(deviceInstance, volumeBuilder);
//    }

//    #region Public Methods

//    internal ICollection<VerificationTestPoint> Build()
//    {
//        return _tests;
//    }

//    public TestPointBuilder AddCorrectionTest(Func<CorrectionBuilder, CorrectionBuilder> testDecoratorFunc)
//    {
//        var testPoint = testDecoratorFunc.Invoke(new CorrectionBuilder(_device, _tests.Count));
//        _current = testPoint.Build();
//        _tests.Add(_current);
//        return this;
//    }

//    public TestPointBuilder WithVolume(Func<VolumeInputBuilder, VolumeInputBuilder> testDecorator = null)
//    {
//        var volumeTests = testDecorator?.Invoke(_volumeBuilder) 
//                ?? _volumeBuilder.AddDefaults(_current);

//        _current.AddTests(volumeTests.Build());
//        return this;
//    }

//    #endregion
//}
