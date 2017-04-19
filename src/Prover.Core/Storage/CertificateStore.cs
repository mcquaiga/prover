using System;
using System.Linq;
using System.Threading.Tasks;
using Prover.Core.Models.Certificates;

namespace Prover.Core.Storage
{
    public interface ICertificateStore
    {
        IQueryable<Certificate> Query();
        Certificate GetCertificate(Guid id);
        Task UpsertAsync(Certificate entity);
    }

    public class CertificateStore : ICertificateStore
    {
        private readonly ProverContext _proverContext;

        public CertificateStore(ProverContext proverContext)
        {
            _proverContext = proverContext;
        }

        public IQueryable<Certificate> Query()
        {
            return _proverContext.Certificates.AsQueryable();
        }

        public Certificate GetCertificate(Guid id)
        {
            return _proverContext.Certificates.Find(id);
        }

        public async Task UpsertAsync(Certificate entity)
        {
            _proverContext.Certificates.Add(entity);
            await _proverContext.SaveChangesAsync();
        }     
    }
}