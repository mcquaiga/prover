using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using DynamicData;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.ViewModels;

namespace Prover.Application.Interfaces
{
    public interface IVerificationTestService
    {
        Task<EvcVerificationViewModel> AddOrUpdate(EvcVerificationViewModel viewModel);
        Task<EvcVerificationTest> AddOrUpdate(EvcVerificationTest evcVerificationTest);

        EvcVerificationTest CreateModel(EvcVerificationViewModel viewModel);

        IObservableCache<EvcVerificationTest, Guid> FetchTests();

        Task<EvcVerificationViewModel> GetViewModel(
            EvcVerificationTest verificationTest);

        Task<ICollection<EvcVerificationViewModel>> GetViewModel(
            IEnumerable<EvcVerificationTest> verificationTests);

        EvcVerificationViewModel NewVerification(DeviceInstance device, VerificationTestOptions options = null);
        EvcVerificationTest NewVerificationModel(DeviceInstance device, VerificationTestOptions options = null);
        Task AddOrUpdateBatch(IEnumerable<EvcVerificationTest> evcVerificationTest);

    }
}