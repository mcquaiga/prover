using Prover.Shared;

namespace Prover.Domain.EvcVerifications.Verifications.Volume.InputTypes
{
    public interface IVolumeInputType
    {
        #region Public Properties

        VolumeInputType InputType { get; }

        #endregion

        #region Public Methods

        int MaxUncorrectedPulses();

        decimal UnCorrectedInputVolume(decimal appliedInput);

       
        #endregion
    }

    
}