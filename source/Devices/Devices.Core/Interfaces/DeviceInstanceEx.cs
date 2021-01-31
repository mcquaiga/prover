using System;
using System.Collections.Generic;
using System.Linq;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Prover.Shared;

namespace Devices.Core.Interfaces
{
	public static class DeviceInstanceEx
	{
		public static DeviceInstance CreateInstance(this DeviceType deviceType,
				IDictionary<string, string> itemValuesDictionary)
		{
			var items = deviceType.ToItemValues(itemValuesDictionary);
			return deviceType.Factory.CreateInstance(items);
		}

		public static DeviceInstance CreateInstance(this DeviceType deviceType,
				IEnumerable<ItemValue> itemValues)
		{
			return deviceType.Factory.CreateInstance(itemValues);
		}

		public static DeviceInstance CreateInstance(this DeviceType deviceType,
				IDictionary<int, string> itemValuesDictionary)
		{
			var items = deviceType.ToItemValues(itemValuesDictionary);
			return deviceType.Factory.CreateInstance(items);
		}


		public static IEnumerable<ItemValue> CombineValuesWithItemFile(this DeviceInstance deviceInstance, IEnumerable<ItemValue> itemValues) => itemValues.Union(deviceInstance.Values);

		public static string CompanyNumber(this DeviceInstance device) => device.Items.SiteInfo.SiteId2;

		public static IEnumerable<ItemValue> CreateGroupItemValues<TGroup>(this DeviceInstance device, IEnumerable<ItemValue> itemValues) where TGroup : ItemGroup
		{
			var values = itemValues.ToList();

			//var joined = device.Values.Except(values, new ItemValueComparer()).ToList();
			var joined = values.Union(device.Values);
			return joined;
		}

		public static TGroup CreateItemGroup<TGroup>(this DeviceInstance device, IEnumerable<ItemValue> itemValues) where TGroup : ItemGroup => device.CreateItemGroup<TGroup>(itemValues);


		public static TGroup CreateItemGroup<TGroup>(this DeviceInstance device, Dictionary<string, string> itemValues) where TGroup : ItemGroup
		{
			return device.CreateItemGroup<TGroup>(device.DeviceType.ToItemValues(itemValues));
		}

		public static TGroup CreateItemGroup<TGroup>(this DeviceInstance device, Dictionary<int, string> itemValues) where TGroup : ItemGroup
		{
			return device.CreateItemGroup<TGroup>(device.DeviceType.ToItemValues(itemValues));
		}

		public static ItemGroup GetGroupValues(this DeviceInstance deviceInstance, IEnumerable<ItemValue> itemValues, Type groupType)
		{
			var combinedValues = deviceInstance.CombineValuesWithItemFile(itemValues);
			return deviceInstance.DeviceType.GetGroupValues(combinedValues, groupType);
		}

		public static ItemMetadata GetItemMetadata(this DeviceType deviceType, int itemId)
		{

			return deviceType.Items.FirstOrDefault(i => i.Number == itemId);
		}


		public static bool HasLivePressure(this DeviceInstance device) => device.Composition() == CompositionType.P || device.Composition() == CompositionType.PTZ;

		public static bool HasLiveSuper(this DeviceInstance device) => device.Composition() == CompositionType.PTZ;

		public static bool HasLiveTemperature(this DeviceInstance device) => device.Composition() == CompositionType.T || device.Composition() == CompositionType.PTZ;

		public static PressureItems Pressure(this DeviceInstance device) => device.ItemGroup<PressureItems>();
		public static PulseOutputItems PulseOutput(this DeviceInstance device) => device.ItemGroup<PulseOutputItems>();

		public static ItemValue SetItemValue(this DeviceInstance device, ItemMetadata itemMetadata, string value)
		{
			var itemValue = ItemValue.Create(itemMetadata, value);
			device.SetItemValues(new[] { itemValue });
			return itemValue;
		}

		public static ItemValue SetItemValue(this DeviceInstance device, int itemId, string value)
		{
			var item = device.DeviceType.GetItemMetadata(itemId);
			return device.SetItemValue(item, value);
		}

		public static SiteInformationItems SiteInfo(this DeviceInstance device) => device.ItemGroup<SiteInformationItems>();
		public static SuperFactorItems SuperFactor(this DeviceInstance device) => device.ItemGroup<SuperFactorItems>();
		public static TemperatureItems Temperature(this DeviceInstance device) => device.ItemGroup<TemperatureItems>();
		public static VolumeItems Volume(this DeviceInstance device) => device.ItemGroup<VolumeItems>();
	}

	public static class SiteInformationEx
	{
		public static CompositionType Composition(this DeviceInstance device) => device.ItemGroup<SiteInformationItems>()
																					   .CompositionType;

		public static CompositionType Composition(this SiteInformationItems siteInfo) => siteInfo.CompositionType;

		public static string CompositionDescription(this SiteInformationItems siteInfo)
		{
			switch (siteInfo.Composition())
			{
				case CompositionType.T:
					return "Temperature";
				case CompositionType.P:
					return "Pressure";
				case CompositionType.PTZ:
					return "Pressure & Temperature";
				case CompositionType.Fixed:
					return "Fixed";
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public static string CompositionShort(this DeviceInstance device) => device.Composition()
																				   .ToString();

		public static string CompositionShort(this SiteInformationItems siteInfo) => siteInfo.Composition()
																							 .ToString();

		public static string InventoryNumber(this SiteInformationItems siteInfo) => siteInfo.SiteId2;
	}
}