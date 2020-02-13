using System;
using System.Collections.Generic;
using Devices.Core.Interfaces;
using Domain.EvcVerifications.Verifications.Volume.InputTypes;

namespace Application.ViewModels
{
    public class EvcVerificationViewModel : BaseViewModel
    {
        #region Public Properties

        public DeviceInstance Device { get; set; }

        public DeviceType DeviceType { get; set; }

        public IVolumeInputType DriveType { get; set; }

        public DateTimeOffset TestDateTime { get; set; }

        public ICollection<VerificationTestPointViewModel> Tests { get; set; }
        #endregion
    }
}