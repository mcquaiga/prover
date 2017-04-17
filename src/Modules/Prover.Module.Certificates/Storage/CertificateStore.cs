using System;
using System.Linq;
using System.Threading.Tasks;
using Prover.Core.Storage;
using Prover.Modules.Certificates.Models;

namespace Prover.Modules.Certificates.Storage
{
    public class CertificateStore : ICertificateStore<Certificate>
    {
        private readonly CertificateContext _certificateContext;

        public CertificateStore(CertificateContext certificateContext)
        {
            _certificateContext = certificateContext;
        }

        public IQueryable<Certificate> Query()
        {
            return _certificateContext.Certificates.AsQueryable();
        }

        public async Task<Certificate> UpsertAsync(Certificate entity)
        {
            _certificateContext.Certificates.Add(entity);
            await _certificateContext.SaveChangesAsync();

            return entity;
        }

        public void Dispose()
        {
        }

        public Certificate Get(Guid id)
        {
            return _certificateContext.Certificates.Find(id);
        }

        public Task Delete(Certificate entity)
        {
            throw new NotImplementedException();
        }
    }
}