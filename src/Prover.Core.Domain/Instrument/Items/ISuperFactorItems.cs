namespace Prover.Domain.Instrument.Items
{
    public interface ISuperFactorItems : IItemsGroup
    {
        double Co2 { get; }
        double N2 { get; }
        double SpecGr { get; }
    }
}