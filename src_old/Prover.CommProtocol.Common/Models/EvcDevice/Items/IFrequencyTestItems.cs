namespace Prover.CommProtocol.Common.Items
{
    public interface IFrequencyTestItems
    {
        decimal TibAdjustedVolumeReading { get; set; }
        long TibUnadjustedVolumeReading { get; }
        
        long MainUnadjustVolumeReading { get; set; }
        long MainAdjustedVolumeReading {get; set;}
    }
}