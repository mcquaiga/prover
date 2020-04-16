using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using DynamicData;
using Prover.Application.ViewModels;
using Prover.Domain.EvcVerifications;

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

        EvcVerificationViewModel NewVerification(DeviceInstance device);
        Task AddOrUpdateBatch(IEnumerable<EvcVerificationTest> evcVerificationTest);

    }
}