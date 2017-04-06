using Prover.Shared.Enums;

namespace Prover.Domain.Models.Instruments.Items
{
    public interface ITemperatureItems
    {
        decimal Base { get; }
        TemperatureUnits Units { get; }
        decimal GasTemperature { get; }
        decimal Factor { get; }        
    }
}