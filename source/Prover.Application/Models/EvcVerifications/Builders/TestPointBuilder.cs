using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Prover.Application.Models.EvcVerifications.Verifications.CorrectionFactors;
using Prover.Calculations;

namespace Prover.Application.Models.EvcVerifications.Builders
{
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

    public class TestPointBuilder
    {
        public readonly DeviceInstance Device;
        private readonly ICollection<ItemValue> _deviceValues;
        private readonly VerificationTestPoint _testPoint;
        private readonly VolumeInputTestBuilder _volumeBuilder;

        internal TestPointBuilder(DeviceInstance device, int testsCount, VolumeInputTestBuilder volumeBuilder, ICollection<ItemValue> deviceValues = null)
        {
            Device = device;
            _volumeBuilder = volumeBuilder;
            _deviceValues = deviceValues ?? Device.Values;
            _testPoint = new VerificationTestPoint(testsCount);
        }

        public VerificationTestPoint Build() => _testPoint;

        public TestPointBuilder WithPressure(decimal gauge, decimal? atmGauge, PressureItems pressureItems = null)
        {
            if (Device.HasLivePressure())
            {
                pressureItems = pressureItems ?? Device.CreateItemGroup<PressureItems>(_deviceValues);
                _testPoint.AddTests(new PressureCorrectionTest(pressureItems, gauge, atmGauge ?? pressureItems.AtmosphericPressure));
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

                _testPoint.AddTests(new SuperCorrectionTest(si, temp, pressure));
            }

            return this;
        }

        public TestPointBuilder WithTemperature(decimal gaugeTemp, TemperatureItems temperatureItems = null)
        {
            if (Device.HasLiveTemperature())
            {
                temperatureItems = temperatureItems ?? Device.CreateItemGroup<TemperatureItems>(_deviceValues);
                _testPoint.AddTests(new TemperatureCorrectionTest(temperatureItems, gaugeTemp));
            }

            return this;
        }

        public TestPointBuilder WithVolume
                (VolumeItems startValues = null, VolumeItems endValues = null, Func<VolumeInputTestBuilder, VolumeInputTestBuilder> testDecorator = null,
                int appliedInput = 0, int corPulses = 0, int uncorPulses = 0)
        {
            _volumeBuilder.SetItemValues(startValues, endValues, appliedInput, corPulses, uncorPulses);

            var volumeTests = testDecorator?.Invoke(_volumeBuilder) 
                    ?? _volumeBuilder.AddDefaults(_testPoint);
            
            _testPoint.AddTests(volumeTests.Build());
            
            return this;
        }
    }

    public static class TestPointBuilderEx
    {
        public static TestPointBuilder Generate(this TestPointBuilder builder, decimal gaugePressure, decimal temperatureGauge, Dictionary<string, string> values = null)
        {
            return builder.Generate(gaugePressure, temperatureGauge, builder.Device.DeviceType.ToItemValues(values).ToList());
        }

        public static TestPointBuilder Generate(this TestPointBuilder builder, decimal gaugePressure, decimal temperatureGauge, ICollection<ItemValue> values = null)
        {
            var deviceInstance = builder.Device;
            
            if (deviceInstance.HasLivePressure())
            {
                builder.WithPressure(gaugePressure, deviceInstance.Items.Pressure.AtmosphericPressure, builder.Device.CreateItemGroup<PressureItems>(values));
            }

            if (deviceInstance.HasLiveTemperature())
                builder.WithTemperature(temperatureGauge, builder.Device.CreateItemGroup<TemperatureItems>(values));

            if (deviceInstance.HasLiveSuper() 
                    || (deviceInstance.HasLivePressure() && deviceInstance.HasLiveTemperature()))
                builder.WithSuperFactor(builder.Device.CreateItemGroup<SuperFactorItems>(values));

          

            return builder;
            //return builder.WithSuperFactor(builder.Device.CreateItemGroup<SuperFactorItems>(values));
        }

        public static TestPointBuilder WithPtz(this TestPointBuilder builder, TemperatureItems temperatureItems, decimal temperatureGauge, PressureItems pressureItems, decimal pressureGauge, decimal? atmGauge, SuperFactorItems superItems
        )
            => builder.WithPressure(pressureGauge, atmGauge, pressureItems)
                      .WithTemperature(temperatureGauge, temperatureItems)
                      .WithSuperFactor(superItems);

        public static TestPointBuilder WithPtz(this TestPointBuilder builder, ICollection<ItemValue> itemValues, decimal temperatureGauge, decimal pressureGauge, decimal? atmGauge) => builder.WithPressure(pressureGauge, atmGauge, builder.Device.CreateItemGroup<PressureItems>(itemValues))
                                                                                                                                                                                .WithSuperFactor(builder.Device.CreateItemGroup<SuperFactorItems>(itemValues));
        public static TestPointBuilder WithPtz(this TestPointBuilder builder, decimal temperatureGauge, decimal pressureGauge, decimal? atmGauge) => builder.WithPressure(pressureGauge, atmGauge).WithTemperature(temperatureGauge).WithSuperFactor();
        
        public static TestPointBuilder WithTemperature(this TestPointBuilder builder, decimal gauge, Dictionary<string, string> values) => builder.WithTemperature(gauge, builder.Device.CreateItemGroup<TemperatureItems>(values));
        
        public static TestPointBuilder WithTemperature(this TestPointBuilder builder, decimal gauge, Dictionary<int, string> values) => builder.WithTemperature(gauge, builder.Device.CreateItemGroup<TemperatureItems>(values));
        
        public static TestPointBuilder WithPressure(this TestPointBuilder builder, decimal gauge, decimal atm, Dictionary<int, string> values) => builder.WithPressure(gauge,atm, builder.Device.CreateItemGroup<PressureItems>(values));
        public static TestPointBuilder WithPressure(this TestPointBuilder builder, decimal gauge, decimal atm, Dictionary<string, string> values) => builder.WithPressure(gauge, atm, builder.Device.CreateItemGroup<PressureItems>(values));
        public static TestPointBuilder WithSuperFactor(this TestPointBuilder builder, Dictionary<string, string> values) => builder.WithSuperFactor(builder.Device.CreateItemGroup<SuperFactorItems>(values));
        public static TestPointBuilder WithSuperFactor(this TestPointBuilder builder, Dictionary<int, string> values) => builder.WithSuperFactor(builder.Device.CreateItemGroup<SuperFactorItems>(values));
        public static TestPointBuilder WithVolume(this TestPointBuilder builder, Dictionary<string, string> startValues, Dictionary<string, string> endValues) => builder.WithVolume(builder.Device.CreateItemGroup<VolumeItems>(startValues), builder.Device.CreateItemGroup<VolumeItems>(endValues));
        public static TestPointBuilder WithVolume(this TestPointBuilder builder, Dictionary<string, string> startValues, Dictionary<string, string> endValues, int appliedInput, int correctedPulses, int uncorrectedPulses) 
            => builder.WithVolume(builder.Device.CreateItemGroup<VolumeItems>(startValues), builder.Device.CreateItemGroup<VolumeItems>(endValues), appliedInput: appliedInput, corPulses: correctedPulses, uncorPulses:uncorrectedPulses);
        
        


    }
}