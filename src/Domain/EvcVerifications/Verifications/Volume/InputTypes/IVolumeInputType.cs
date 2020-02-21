using System;
using System.Collections.Generic;
using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Domain.EvcVerifications.Builders;

namespace Domain.EvcVerifications.Verifications.Volume.InputTypes
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