namespace Devices.Core.Interfaces.Items
{
    public interface ISuperFactorItems : IItemsGroup
    {
        #region Public Properties

        double Co2 { get; set; }
        double N2 { get; set; }
        double SpecGr { get; set; }

        #endregion Public Properties
    }
}