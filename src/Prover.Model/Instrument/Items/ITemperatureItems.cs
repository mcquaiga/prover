using Prover.Shared.Enums;

namespace Prover.Domain.Instrument.Items
{
    public interface ITemperatureItems : IItemsGroup
    {
        double Base { get; set; }
        double Factor { get; set; }
        double GasTemperature { get; set; }
        TemperatureUnits Units { get; set; }
    }
}