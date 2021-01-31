using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Prover.Calculations;

namespace Prover.Application.Models.EvcVerifications.Builders
{

	public static class VerificationBuilderEx
	{
		public static VerificationBuilder BuildVerification(this DeviceInstance deviceInstance)
		{
			return VerificationBuilder.CreateNew(deviceInstance);
		}

		public static EvcVerificationTest NewVerification(this DeviceInstance deviceInstance, VerificationTestOptions options = null)
		{
			options = options ?? VerificationDefaults.VerificationOptions;

			var builder = VerificationBuilder.CreateNew(deviceInstance);

			options.CorrectionTestDefinitions.ForEach(def =>
			{
				builder.AddTestPoint(tp =>
				{
					if (deviceInstance.HasLivePressure())
					{
						var gaugePressure = PressureCalculator.GetGaugePressure(deviceInstance.Items.Pressure.Range, def.PressureGaugePercent);
						tp.WithPressure(gaugePressure, deviceInstance.Items.Pressure.AtmosphericPressure);
					}

					if (deviceInstance.HasLiveTemperature())
						tp.WithTemperature(def.TemperatureGauge);

					if (deviceInstance.HasLiveSuper()
							|| (deviceInstance.HasLivePressure() && deviceInstance.HasLiveTemperature()))
						tp.WithSuperFactor();

					if (def.IsVolumeTest)
						tp.WithVolume();

					return tp;
				});
			});

			return builder.Build();
		}

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

		public static TestPointBuilder WithPressure(this TestPointBuilder builder, decimal gauge, decimal atm, Dictionary<int, string> values) => builder.WithPressure(gauge, atm, builder.Device.CreateItemGroup<PressureItems>(values));
		public static TestPointBuilder WithPressure(this TestPointBuilder builder, decimal gauge, decimal atm, Dictionary<string, string> values) => builder.WithPressure(gauge, atm, builder.Device.CreateItemGroup<PressureItems>(values));
		public static TestPointBuilder WithSuperFactor(this TestPointBuilder builder, Dictionary<string, string> values) => builder.WithSuperFactor(builder.Device.CreateItemGroup<SuperFactorItems>(values));
		public static TestPointBuilder WithSuperFactor(this TestPointBuilder builder, Dictionary<int, string> values) => builder.WithSuperFactor(builder.Device.CreateItemGroup<SuperFactorItems>(values));
		public static TestPointBuilder WithVolume(this TestPointBuilder builder, Dictionary<string, string> startValues, Dictionary<string, string> endValues) => builder.WithVolume(builder.Device.ToItemValues(startValues), builder.Device.ToItemValues(endValues));
		public static TestPointBuilder WithVolume(this TestPointBuilder builder, Dictionary<string, string> startValues, Dictionary<string, string> endValues, int appliedInput, int correctedPulses, int uncorrectedPulses)
			=> builder.WithVolume(builder.Device.ToItemValues(startValues), builder.Device.ToItemValues(endValues), appliedInput: appliedInput, corPulses: correctedPulses, uncorPulses: uncorrectedPulses);
	}
}