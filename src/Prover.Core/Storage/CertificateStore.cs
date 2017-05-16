using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;

namespace Prover.Core.Storage
{
    public interface ICertificateStore
    {
        IQueryable<Certificate> Query();
        Certificate GetCertificate(Guid id);
        Task<Certificate> GetCertificate(long number);
        Task UpsertAsync(Certificate entity);
        Task<long> GetNextCertificateNumber();
        Task<Certificate> CreateCertificate(string testedBy, string verificationType, List<Instrument> instruments);
    }

    public class CertificateStore : ICertificateStore
    {
        private readonly ProverContext _proverContext;
        private readonly IProverStore<Instrument> _instrumentStore;

        public CertificateStore(ProverContext proverContext, IProverStore<Instrument> instrumentStore)
        {
            _proverContext = proverContext;
            _instrumentStore = instrumentStore;
        }

        public IQueryable<Certificate> Query()
        {
            return _proverContext.Certificates
                .Include(c => c.Instruments)
                .AsQueryable();
        }

        public async Task<Certificate> GetCertificate(long number)
        {
            var cert = await Query().FirstOrDefaultAsync(x => x.Number == number);
            if (cert == null) return null;

            var instruments = cert.Instruments
                .Select(i => _instrumentStore.Get(i.Id)).ToList();

            cert.Instruments = instruments;

            return cert;
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

        public async Task<long> GetNextCertificateNumber()
        {
            var last = await Query()
                .Select(x => x.Number)
                .OrderByDescending(x => x)
                .FirstOrDefaultAsync();

            return last + 1;
        }

        public async Task<Certificate> CreateCertificate(string testedBy, string verificationType, List<Instrument> instruments)
        {
            var certificate = new Certificate
            {
                CreatedDateTime = DateTime.Now,
                VerificationType = verificationType,
                TestedBy = testedBy,
                Number = await GetNextCertificateNumber(),
                Instruments = new Collection<Instrument>()
            };

            instruments.ForEach(i =>
            {
                i.CertificateId = certificate.Id;
                i.Certificate = certificate;
                certificate.Instruments.Add(i);
            });

            await UpsertAsync(certificate);
            return certificate;
        }
    }
}