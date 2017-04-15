namespace Prover.Domain.Instrument.Items
{
    public interface ISuperFactorItems : IItemsGroup
    {
        double Co2 { get; set; }
        double N2 { get; set; }
        double SpecGr { get; set; }
    }
}