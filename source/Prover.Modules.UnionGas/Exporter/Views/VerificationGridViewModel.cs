using System;
using System.Collections.Generic;
using System.Text;
using Devices.Core.Interfaces;
using Prover.Application.ViewModels;
using Prover.Domain.EvcVerifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Modules.UnionGas.Exporter.Views
{
    public class VerificationGridViewModel : ReactiveObject
    {
        [Reactive] public EvcVerificationTest Test { get; private set; }

        public VerificationGridViewModel(EvcVerificationTest verificationTest)
        {
            Test = verificationTest;
            DeviceInfo = new DeviceInfoViewModel(verificationTest.Device);
        }

        [Reactive] public DeviceInfoViewModel DeviceInfo { get; private set; }

        public string CompositionType => Test.Device.CompositionString();
    }
}
