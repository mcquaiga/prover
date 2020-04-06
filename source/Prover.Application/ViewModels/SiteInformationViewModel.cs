using System;
using Devices.Core.Interfaces;

namespace Prover.Application.ViewModels
{
    public class SiteInformationViewModel : DeviceInfoViewModel
    {
        private DateTime _startTestDate;
        public EvcVerificationViewModel Test { get; }

        public SiteInformationViewModel(DeviceInstance device, EvcVerificationViewModel verificationViewModel) : base(device)
        {
            Test = verificationViewModel;
            _startTestDate = DateTime.Now;
        }

        public DateTime TestDateTime => Test.TestDateTime ?? _startTestDate;
    }
}