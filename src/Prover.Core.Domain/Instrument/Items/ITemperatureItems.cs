using Prover.Shared.Enums;

namespace Prover.Domain.Instrument.Items
{
    public interface ITemperatureItems
    {
        decimal Base { get; }
        decimal Factor { get; }
        decimal GasTemperature { get; }
        TemperatureUnits Units { get; }
    }
}