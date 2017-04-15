using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prover.Domain.Model.Verifications;

namespace Prover.Services.Verification
{
    public interface IVerificationRunProviderService
    {
        Task CreateVerificationRun(VerificationRun verificationRun);
        Task<VerificationRun> GetVerificationRun(Guid id);
        Task<IEnumerable<VerificationRun>> GetVerificationRunsNotExported();
        Task<IEnumerable<VerificationRun>> GetVerificationRunsArchived();
        Task<IEnumerable<VerificationRun>> GetVerificationRuns();
        Task<bool> SaveVerificationRun(VerificationRun verificationRun);
        Task<bool> ArchiveVerification(VerificationRun verificationRun);
    }
}