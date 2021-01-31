using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Prover.Shared.Extensions;

namespace Prover.Application.ValidationRules {

	public static class ItemValidationRules {
		//public static
	}


	public class ItemValidContext {

		private Func<ItemValue> _updateFunc;

		private ItemValidContext() {
		}

		private static ItemValidContext SetResult(bool isValid) => new ItemValidContext() {
			IsValid = isValid
		};

		public static ItemValidContext SetValue(ItemValue updateValue) => new ItemValidContext() {
			IsValid = false,
			_updateFunc = () => updateValue
		};

		public static ItemValidContext SetValue(Func<ItemValue> updateFunc) => new ItemValidContext() {
			IsValid = false,
			_updateFunc = updateFunc
		};
		public static ItemValidContext SetValid() => SetResult(true);

		public bool IsValid { get; private set; }

		public virtual ItemValue GetValue() => _updateFunc();
	}

	public class ItemValidationRule {
		//private readonly Func<DeviceInstance, IItemValueUpdater> _validationFunc;
		//private Action<IItemValueUpdater> _updaterFunc;

		internal ItemValidationRule(Action<DeviceInstance, IItemValueUpdater> validationFunc, Func<ItemValue> getValueFunc) {
			ValidateRule = validationFunc;
			//UpdateWith = getValueFunc;
		}

		protected ItemValidationRule() {

		}

		public void Validate(DeviceInstance device, IItemValueUpdater updater) {

			ValidateRule(device, updater);
			//var isValid = ValidateRule(device);

			//if (isValid)
			//	return ItemValidContext.SetValid();

			//_updaterFunc(updater);
			//updater.SetItemValue(UpdateWith);
			//return ItemValidContext.SetValue(UpdateWith);
		}

		///protected Func<ItemValue> UpdateWith { get; set; }
		protected Action<DeviceInstance, IItemValueUpdater> ValidateRule { get; set; }

		//protected void Update(Func<ItemValue> func) {
		//	UpdateWith = func;
		//}

		//protected void Update(Action<IItemValueUpdater> func) {
		//	_updaterFunc = func;
		//}

		protected void Validate(Action<DeviceInstance, IItemValueUpdater> func) {
			ValidateRule = func;
		}
	}

	public class DateTimeValidationRule : ItemValidationRule {

		public DateTimeValidationRule() {
			this.Validate((device, updater) => {
				var site = device.ItemGroup<SiteInformationItems>();
				var itemMetadata = device.DeviceType.GetItemMetadata<SiteInformationItems>().ToList();

				if (site.Date != DateTime.Now.Date) {
					//updater.SetItemValue(() => ItemValue.Create());
				}
			});

		}
	}
}
