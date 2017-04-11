namespace Prover.Core.DomainPortable.Instrument.Items
{
    public interface ITemperatureItems
    {
        decimal Base { get; }
        decimal Factor { get; }
        decimal GasTemperature { get; }
        TemperatureUnits Units { get; }
    }
}