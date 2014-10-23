using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
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
        private ProverContext _proverContext;
        public CertificateStore(IUnityContainer container)
        {
            _proverContext = container.Resolve<ProverContext>();
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
