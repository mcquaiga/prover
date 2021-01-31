using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Prover.Application.Interfaces;
using Prover.Application.Verifications;
using Prover.Application.ViewModels;

namespace Prover.Application.ValidationRules {

	public interface IItemValueUpdater {
		void SetItemValue(Func<ItemValue> value);
	}

	public class ItemFileValidator<TValue> : IItemValueUpdater, IEventsSubscriber {
		private readonly IDeviceSessionManager _deviceManager;
		private readonly ICollection<ItemValidationRule> _rules;

		protected ItemFileValidator(IDeviceSessionManager deviceManager, ICollection<ItemValidationRule> rules) {
			_deviceManager = deviceManager;
			_rules = rules;
		}

		public async Task<bool> Execute(EvcVerificationViewModel verification) {
			var device = verification.Device;
			var itemValue = GetDeviceValue(device);

			var isValid = await CheckIfValid(device, itemValue);

			while (!isValid) {
				var newValue = await GetUpdatedValue();

				if (newValue == null || (newValue.IsTypeOf(typeof(string)) && string.IsNullOrEmpty(newValue.ToString())))
					return false;

				itemValue.SetValue(newValue);

				isValid =
					await CheckIfValid(device, itemValue);

				if (isValid)
					await UpdateDeviceValue(_deviceManager, device, newValue);
			};

			return true;
		}

		public async Task RunValidationRules(DeviceInstance device) {
			//var contexts = new List<ItemValidationContext>();

			var contexts = _rules.ForEach(rule => rule.Validate(device, this));

			foreach (var rule in _rules) {
				var ctx = await rule.Validate(device);
				if (!ctx.IsValid)
					contexts.Add(ctx);
			}
		}

		protected virtual async Task<TValue> GetUpdatedValue() => (TValue)await Interactions.Messages.GetInput.Handle(InputMessage);

		protected abstract string InputMessage { get; }

		protected abstract ItemValue GetDeviceValue(DeviceInstance device);

		protected abstract Task<bool> CheckIfValid(DeviceInstance device, ItemValue itemValueToValidate);

		protected virtual async Task<bool> UpdateDeviceValue(IDeviceSessionManager deviceManager, DeviceInstance device,
			TValue updateValue) {
			var item = GetDeviceValue(device).Metadata;

			var itemValue = await deviceManager.WriteItemValue(item, updateValue.ToString());
			if (itemValue != null)
				device.SetItemValues(new[] { itemValue });

			return itemValue != null;
		}

		/// <inheritdoc />
		public void SubscribeToEvents() {
			VerificationEvents.OnInitialize.Subscribe(async (input) => { await Execute(input.Input); });
		}


		//public class DeviceInstanceChangeLog : DeviceInstance {

		//}
		/// <inheritdoc />
		public void SetItemValue(ItemValue value) {
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		public void SetItemValue(Func<ItemValue> value) {
			throw new NotImplementedException();
		}
	}
}
