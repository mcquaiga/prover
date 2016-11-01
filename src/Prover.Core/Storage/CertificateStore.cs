using System;
using System.Linq;
using Prover.Core.Models.Certificates;

namespace Prover.Core.Storage
{
    public interface ICertificateStore<T> : IDisposable where T : class
    {
        IQueryable<Certificate> Query();
        Certificate GetCertificate(Guid id);
        void Upsert(Certificate entity);
    }

    public class CertificateStore : ICertificateStore<Certificate>
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

        public void Upsert(Certificate entity)
        {
            _proverContext.Certificates.Add(entity);
            _proverContext.SaveChanges();
        }

        public void Dispose()
        {
        }
    }
}