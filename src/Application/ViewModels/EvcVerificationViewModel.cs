using System;
using System.Collections.Generic;
using Devices.Core.Interfaces;
using Domain.EvcVerifications.DriveTypes;

namespace Application.ViewModels
{
    public class EvcVerificationViewModel
    {
        #region Public Properties

        public Guid Id { get; set; }

        public DeviceInstance Device { get; set; }

        public DeviceType DeviceType { get; set; }

        public IVolumeInputType DriveType { get; set; }

        public DateTimeOffset TestDateTime { get; set; }

        public ICollection<VerificationTestPointViewModel> Tests { get; set; }
        #endregion
    }
}