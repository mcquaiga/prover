namespace Prover.CommProtocol.Common.Items
{
    public interface IFrequencyTestItems
    {
        decimal AdjustedVolumeReading { get; set; }
        long UnadjustVolumeReading { get; set; }

    }
}