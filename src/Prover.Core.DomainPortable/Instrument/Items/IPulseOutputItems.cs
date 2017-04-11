namespace Prover.Core.DomainPortable.Instrument.Items
{
    public interface IPulseOutputItems
    {
        decimal PulseBScaling { get; }
        decimal PulserAScaling { get; }
        string PulserAUnits { get; }

        string PulserBUnits { get; }
    }
}