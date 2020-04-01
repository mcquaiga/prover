using System.Collections.Generic;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using Prover.Domain.EvcVerifications;

namespace Prover.Application.Services
{
    public interface IVerificationTestService
    {
        Task<bool> AddOrUpdate(EvcVerificationViewModel viewModel);
        EvcVerificationTest CreateVerificationTestFromViewModel(EvcVerificationViewModel viewModel);

        Task<EvcVerificationViewModel> GetVerificationTest(
            EvcVerificationTest verificationTest);

        Task<ICollection<EvcVerificationViewModel>> GetVerificationTests(
            IEnumerable<EvcVerificationTest> verificationTests);

        EvcVerificationViewModel NewTest(DeviceInstance device);
        Task<ITestManager> NewTestManager(DeviceType deviceType);
    }
}