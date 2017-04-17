using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prover.Domain.Model.Verifications;
using Prover.Shared.Data;

namespace Prover.Services.Verification
{
    public class VerificationRunProviderService : IVerificationRunProviderService
    {
        private readonly IRepository<VerificationRun> _runRepository;

        public VerificationRunProviderService(IRepository<VerificationRun> runRepository)
        {
            _runRepository = runRepository;
        }

        public async Task CreateVerificationRun(VerificationRun verificationRun)
        {
            if (verificationRun == null)
                throw new ArgumentNullException(nameof(verificationRun));

            await _runRepository.InsertAsync(verificationRun);
        }

        public async Task<VerificationRun> GetVerificationRun(Guid id)
        {
            return await _runRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<VerificationRun>> GetVerificationRunsNotExported()
        {
            return await _runRepository.FindAsync(run => run.ExportedDateTime != null || run.ArchivedDateTime != null);
        }

        public async Task<IEnumerable<VerificationRun>> GetVerificationRunsArchived()
        {
            return await _runRepository.FindAsync(t => t.ArchivedDateTime.HasValue);
        }

        public async Task<IEnumerable<VerificationRun>> GetVerificationRuns()
        {
            return await _runRepository.FindAsync(x => true);
        }

        public async Task<bool> SaveVerificationRun(VerificationRun verificationRun)
        {
            await _runRepository.UpsertAsync(verificationRun);
            return true;
        }

        public async Task<bool> ArchiveVerification(VerificationRun verificationRun)
        {
            var updatedTestRun = await GetVerificationRun(verificationRun.Id);
            if (updatedTestRun != null)
            {
                updatedTestRun.ArchivedDateTime = DateTime.UtcNow;
                await _runRepository.UpdateAsync(verificationRun);
                return true;
            }

            return false;
        }
    }
}