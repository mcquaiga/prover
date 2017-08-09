using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Prover.Core.Models.Certificates;

namespace Prover.Core.Storage
{
    public class CertificateStore : IProverStore<Certificate>
    {
        private readonly ProverContext _proverContext;

        public CertificateStore(ProverContext proverContext)
        {
            _proverContext = proverContext;
        }

        public IQueryable<Certificate> Query()
        {           
            return _proverContext.Certificates
                .Include(c => c.Instruments)
                .AsQueryable();
        }

        public Certificate Get(Guid id)
        {
            return _proverContext.Certificates.Find(id);
        }

        public async Task<Certificate> UpsertAsync(Certificate entity)
        {
            _proverContext.Certificates.Add(entity);
            await _proverContext.SaveChangesAsync();

            return entity;
        }

        public Task Delete(Certificate entity)
        {
            throw new NotImplementedException();
        }
    }
}