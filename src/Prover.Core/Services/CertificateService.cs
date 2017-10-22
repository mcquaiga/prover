using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;

namespace Prover.Core.Services
{
    public interface ICertificateService
    {
        Task<Certificate> GetCertificate(Guid id);
        Task<Certificate> GetCertificate(long number);

        IEnumerable<Instrument> GetInstrumentsWithNoCertificate(Guid? clientId = null, bool showArchived = false);

        Task<long> GetNextCertificateNumber();
        Task<Certificate> CreateCertificate(string testedBy, string verificationType, List<Instrument> instruments);
        IEnumerable<Certificate> GetAllCertificates(Client client);
        IEnumerable<Certificate> GetAllCertificates(Client client, long fromNumber, long toNumber);
        IEnumerable<string> GetDistinctTestedBy();
    }

    public class CertificateService : ICertificateService
    {
        private readonly IProverStore<Certificate> _certificateStore;
        private readonly IProverStore<Instrument> _instrumentStore;

        public CertificateService(IProverStore<Certificate> certificateStore, IProverStore<Instrument> instrumentStore)
        {
            _certificateStore = certificateStore;
            _instrumentStore = instrumentStore;
        }

        public async Task<Certificate> GetCertificate(long number)
        {
            var cert = await _certificateStore.Query().FirstOrDefaultAsync(x => x.Number == number);
            if (cert == null) return null;

            var instruments = cert.Instruments
                .Select(i => _instrumentStore.Get(i.Id)).ToList();

            cert.Instruments = instruments;

            return cert;
        }

        public async Task<Certificate> GetCertificate(Guid id)
        {
            var cert = await _certificateStore.Query().FirstOrDefaultAsync(x => x.Id == id);
            return await GetCertificate(cert.Number);
        }

        public async Task<long> GetNextCertificateNumber()
        {
            var last = await _certificateStore.Query()
                .Select(x => x.Number)
                .OrderByDescending(x => x)
                .FirstOrDefaultAsync();

            return last + 1;
        }

        public async Task<Certificate> CreateCertificate(string testedBy, string verificationType,
            List<Instrument> instruments)
        {
            var client = instruments.First().Client;

            var certificate = new Certificate
            {
                CreatedDateTime = DateTime.Now,
                VerificationType = verificationType,
                TestedBy = testedBy,
                Client = client,
                ClientId = client.Id,
                Number = await GetNextCertificateNumber(),
                Instruments = new Collection<Instrument>()
            };

            instruments.ForEach(i =>
            {
                i.CertificateId = certificate.Id;
                i.Certificate = certificate;
                certificate.Instruments.Add(i);
            });

            await _certificateStore.UpsertAsync(certificate);
            
            return certificate;
        }

        public IEnumerable<Certificate> GetAllCertificates(Client client)
        {
            return GetAllCertificates(client, 0, 0);
        }

        public IEnumerable<Certificate> GetAllCertificates(Client client, long fromNumber, long toNumber)
        {
            return _certificateStore.Query()
                .Where(c => (c.ClientId.HasValue && client.Id != Guid.Empty && c.ClientId.Value == client.Id)
                         && (fromNumber == 0 || c.Number >= fromNumber) && (toNumber == 0 || c.Number <= toNumber))
                .OrderBy(i => i.Number);

        }

        public IEnumerable<string> GetDistinctTestedBy()
        {
            return _certificateStore.Query()                
                .Select(c => c.TestedBy)
                .Distinct();
        }

        public IEnumerable<Instrument> GetInstrumentsWithNoCertificate(Guid? clientId = null, bool showArchived = false)
        {
            try
            {
                var results = _instrumentStore.Query()
                    .Where(x => x.CertificateId == null 
                        && ((clientId == Guid.Empty && x.ClientId == null) || x.ClientId == clientId) 
                        && (showArchived == false && x.ArchivedDateTime == null || showArchived))
                    .OrderBy(x => x.TestDateTime);

                return results;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}