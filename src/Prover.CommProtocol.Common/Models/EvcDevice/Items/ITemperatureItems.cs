namespace Prover.CommProtocol.Common.Models.Instrument.Items
{
    public interface ITemperatureItems : IItemsGroup
    {
        double Base { get; }
        double Factor { get; }
        double GasTemperature { get; }
        TemperatureUnits Units { get; }
    }
}