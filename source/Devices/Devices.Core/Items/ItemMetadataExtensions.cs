using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Core.Interfaces;
using Devices.Core.Items.Descriptions;

namespace Devices.Core.Items
{
	public static class ItemMetadataExtensions
	{
		//public static DeviceInstance CreateInstance(this IDeviceInstanceFactory factory,
		//    IDictionary<int, string> itemValuesDictionary)
		//{
		//    var items = ToItemValuesEnumerable(factory.DeviceType, itemValuesDictionary);
		//    return factory.CreateInstance(items);
		//}




		public static IEnumerable<ItemMetadata> FrequencyTestItems(this IEnumerable<ItemMetadata> items)
		{
			return items.Where(i => i.IsFrequencyTest == true);
		}

		public static IEnumerable<int> GetAllItemNumbers(this IEnumerable<ItemMetadata> items)
		{
			return items.Select(i => i.Number);
		}

		/// <summary>
		///     Returns the item number of the first instance found in the collection
		/// </summary>
		/// <param name="items">Items list</param>
		/// <param name="code">
		///     Code used to identify the item number - these are set in the ItemDefinition.xml files
		/// </param>
		/// <returns></returns>
		public static int GetItem(this IEnumerable<ItemMetadata> items, string code)
		{
			var firstOrDefault = items.FirstOrDefault(i => i.Code == code);

			if (firstOrDefault == null)
				throw new Exception($"Item Code {code} could not be found in item collection.");

			return firstOrDefault.Number;
		}

		public static ItemMetadata GetItem(this IEnumerable<ItemMetadata> items, int itemNumber)
		{
			return items.FirstOrDefault(i => i.Number == itemNumber);
		}

		public static T GetItem<T>(this IEnumerable<ItemMetadata> items, string code) where T : ItemMetadata
		{
			return (T)items.FirstOrDefault(i => i.Code == code);
		}

		public static IEnumerable<ItemDescription> GetItemDescriptions(this IEnumerable<ItemMetadata> items,
			int itemNumber)
		{
			return items.FirstOrDefault(i => i.Number == itemNumber)?.ItemDescriptions;
		}

		public static string GetItemString(this IEnumerable<ItemMetadata> items, Dictionary<int, string> itemValues,
			string code)
		{
			if (itemValues == null || !itemValues.Any())
				throw new ArgumentNullException(nameof(itemValues));

			var itemNumber = items.GetItem(code);

			var result = itemValues.FirstOrDefault(x => x.Key == itemNumber).Value;
			if (result == null)
				throw new KeyNotFoundException($"Item value for {code} and #{itemNumber} not found.");

			return result;
		}

		public static decimal GetItemValue(this IEnumerable<ItemMetadata> items, Dictionary<int, string> itemValues,
			string code)
		{
			var result = items.GetItemString(itemValues, code);
			return Decimal.Parse(result);
		}

		public static IEnumerable<ItemMetadata> PressureItems(this IEnumerable<ItemMetadata> items)
		{
			return items.Where(i => i.IsPressureTest == true);
		}

		public static IEnumerable<ItemMetadata> PulseOutputItems(this IEnumerable<ItemMetadata> items)
		{
			return items.Where(i => i.Number == 5 || i.Number == 6 || i.Number == 7);
		}

		public static IEnumerable<ItemMetadata> TemperatureItems(this IEnumerable<ItemMetadata> items)
		{
			return items.Where(i => i.IsTemperatureTest == true);
		}

		public static IEnumerable<ItemValue> ToItemValues(this DeviceType deviceType,
			IDictionary<int, string> itemValuesDictionary)
		{
			return deviceType.Items.Join(itemValuesDictionary,
				x => x.Number,
				y => y.Key,
				(im, value) => ItemValue.Create(im, value.Value));
		}

		public static IEnumerable<ItemValue> ToItemValues(this DeviceType deviceType,
			IDictionary<string, string> itemValuesDictionary)
		{
			var dict = itemValuesDictionary.ToDictionary(k => Int32.Parse(k.Key), v => v.Value);
			return deviceType.ToItemValues(dict);
		}

		public static ICollection<ItemValue> ToItemValues(this DeviceInstance device,
			IDictionary<string, string> itemValuesDictionary)
		{
			var dict = itemValuesDictionary.ToDictionary(k => Int32.Parse(k.Key), v => v.Value);
			return device.DeviceType.ToItemValues(dict).ToList();
		}

		public static bool TryGetItem(this IEnumerable<ItemMetadata> items, string code, out int number)
		{
			number = -1;
			var firstOrDefault = items.FirstOrDefault(i => i.Code == code);

			if (firstOrDefault == null)
				return false;

			number = firstOrDefault.Number;
			return true;
		}

		public static bool TryGetItem(this IEnumerable<ItemMetadata> items, string code, out ItemMetadata item)
		{
			item = items.FirstOrDefault(i => i.Code == code);

			if (item == null)
				return false;

			return true;
		}

		public static IEnumerable<ItemMetadata> VolumeItems(this IEnumerable<ItemMetadata> items)
		{
			return items.Where(i => i.IsVolumeTest == true);
		}

		public static ItemMetadata GetLivePressureItem(this DeviceType deviceType)
		{
			return deviceType.Items.FirstOrDefault(i => i.IsLiveReadPressure == true);
		}

		public static ItemMetadata GetLivePressureItem(this DeviceInstance device)
		{
			return device.DeviceType.GetLivePressureItem();
		}

		public static ItemMetadata GetLiveTemperatureItem(this DeviceType deviceType)
		{
			return deviceType.Items.FirstOrDefault(i => i.IsLiveReadTemperature == true);
		}

		public static ItemMetadata GetLiveTemperatureItem(this DeviceInstance device)
		{
			return device.DeviceType.GetLiveTemperatureItem();
		}
	}
}