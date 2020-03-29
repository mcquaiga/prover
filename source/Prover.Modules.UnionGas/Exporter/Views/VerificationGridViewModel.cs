using Devices.Core.Interfaces;
using Prover.Application.ViewModels;
using Prover.Domain.EvcVerifications;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Modules.UnionGas.Exporter.Views
{
    public class VerificationGridViewModel : ReactiveObject
    {
        public VerificationGridViewModel(EvcVerificationTest verificationTest)
        {
            Test = verificationTest;
            DeviceInfo = new DeviceInfoViewModel(verificationTest.Device);
        }

        public EvcVerificationTest Test { get; }

        public DeviceInfoViewModel DeviceInfo { get; }

        public string CompositionType => Test.Device.CompositionShort();
    }
}