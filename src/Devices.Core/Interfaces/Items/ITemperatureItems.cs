namespace Devices.Core.Interfaces.Items
{
    public interface ITemperatureItems : IItemsGroup
    {
        #region Public Properties

        double Base { get; }

        double Factor { get; }

        double GasTemperature { get; }

        TemperatureUnits Units { get; }

        #endregion Public Properties
    }
}