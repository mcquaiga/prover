namespace Prover.Domain.Models.Instruments.Items
{
    public interface IPulseOutputItems
    {
        string PulserAUnits { get; }
        decimal PulserAScaling { get; }

        string PulserBUnits { get; }
        decimal PulseBScaling { get; }
    }
}