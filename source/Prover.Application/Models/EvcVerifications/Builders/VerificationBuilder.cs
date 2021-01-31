using Devices.Core.Interfaces;
using Devices.Core.Items;
using System;
using System.Collections.Generic;

namespace Prover.Application.Models.EvcVerifications.Builders
{
	public class VerificationBuilder
	{
		private readonly DeviceInstance _device;
		private EvcVerificationTest _instance;
		private VerificationBase _verification;
		//private readonly VolumeInputTestBuilder _volumeBuilder;
		//private TestPointBuilder _testPointBuilder;

		private VerificationBuilder(DeviceInstance device)
		{
			_device = device;

			//_testPointBuilder = TestPointBuilder.Create(_device, _volumeBuilder);

			_instance = new EvcVerificationTest(device);
			_verification = new VerificationBase(device);
			_instance.Verification = _verification;

			_instance.Prover = VerificationDefaults.ProvingApparatus;
		}

		#region Public Methods

		public static VerificationBuilder CreateNew(DeviceInstance device)
		{
			return new VerificationBuilder(device);
			//.CreateVerificationTests(_instance);
		}

		public EvcVerificationTest Build()
		{
			_verification.AddChildren(_instance.Tests);

			var instance = _instance;
			_instance = null;
			return instance;
		}

		public VerificationBuilder AddTestPoint(Func<TestPointBuilder, TestPointBuilder> testDecoratorFunc, ICollection<ItemValue> deviceValues = null)
		{
			var correctionTests = testDecoratorFunc
					.Invoke(new TestPointBuilder(_device, _instance.Tests.Count, deviceValues ?? new List<ItemValue>()))
					.Build();

			_instance.AddChild(correctionTests);
			return this;
		}

		#endregion

	}


}