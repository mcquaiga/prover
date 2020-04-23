using Devices.Core.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prover.Application.Interfaces
{
    public interface IVerificationTestService
    {
        Task<EvcVerificationViewModel> Save(EvcVerificationViewModel viewModel);
        Task<EvcVerificationTest> Upsert(EvcVerificationTest evcVerificationTest);

        Task<EvcVerificationTest> SubmitVerification(EvcVerificationViewModel viewModel);
        Task<EvcVerificationTest> Archive(EvcVerificationTest model);

        EvcVerificationTest CreateModel(EvcVerificationViewModel viewModel);

        EvcVerificationViewModel NewVerification(DeviceInstance device, VerificationTestOptions options = null);
        /*
                EvcVerificationTest NewVerificationModel(DeviceInstance device, VerificationTestOptions options = null);
        */
        Task UpsertBatch(IEnumerable<EvcVerificationTest> evcVerificationTest);

    }
}