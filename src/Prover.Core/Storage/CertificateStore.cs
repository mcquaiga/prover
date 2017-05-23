using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Prover.Core.Exports;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;

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