namespace Devices.Honeywell.Core.Items.ItemGroups
{
    //public class PressureItemsBuilder : ItemGroupBuilderBase<PressureItems>, IBuildItemsFor<PressureItems>
    //{
    //    #region Public Methods

    //    public PressureItemsBuilder() 
    //    {
    //    }

    //    public PressureItems Build(DeviceType device, IEnumerable<ItemValue> values)
    //    {
    //        var items = values.ToList();

    //        var site = device.GetGroupValues<SiteInformationItems>(items);
    //        if (site.LivePressureFactor == CorrectionFactorType.Fixed) return default;

    //        var p = new PressureItemsHoneywell();
    //        return SetValues(p, items);
    //    }

    //    #endregion
    //}
}