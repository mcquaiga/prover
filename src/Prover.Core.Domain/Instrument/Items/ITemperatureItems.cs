using Prover.Shared.Enums;

namespace Prover.Domain.Instrument.Items
{
    public interface ITemperatureItems : IItemsGroup
    {
        double Base { get; }
        double Factor { get; }
        double GasTemperature { get; }
        TemperatureUnits Units { get; }
    }
}