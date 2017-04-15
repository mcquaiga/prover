namespace Prover.Domain.Instrument.Items
{
    public interface IPulseOutputItems : IItemsGroup
    {
        decimal PulseBScaling { get; }
        decimal PulserAScaling { get; }
        string PulserAUnits { get; }

        string PulserBUnits { get; }
    }
}