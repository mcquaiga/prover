namespace Prover.CommProtocol.Common.Items
{
    public interface IFrequencyTestItems
    {
        decimal TibAdjustedVolumeReading { get; set; }
        long TibUnadjustedVolumeReading { get; }
        
        long UnadjustVolumeReading { get; set; }
        long MainAdjustedVolumeReading {get; set;}
    }
}