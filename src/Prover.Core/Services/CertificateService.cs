using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Settings;
using Prover.Core.Shared.Data;
using Prover.Core.Storage;

namespace Prover.Core.Services
{
    public interface ICertificateService
    {
        Task<Certificate> GetCertificate(Guid id);
        Task<Certificate> GetCertificate(long number);
        Task<List<Instrument>> GetInstrumentsWithNoCertificate(Guid? clientId = null, bool showArchived = false);
        Task<long> GetNextCertificateNumber();
        Task<Certificate> CreateCertificate(string testedBy, string verificationType, List<Instrument> instruments);
        Task<List<Certificate>> GetAllCertificates(Client client);
        Task<List<Certificate>> GetAllCertificates(Client client, long fromNumber, long toNumber);
        Task<List<Instrument>> GetInstrumentsOnCertificate(Certificate cert);
        Task<List<long>> GetCertificateNumbers(Client client);
        IEnumerable<string> GetDistinctTestedBy();
    }

    public class CertificateService : ICertificateService
    {
        private readonly IDbContextScopeFactory _dbContextScopeFactory;
        private readonly IProverStore<Certificate> _certificateStore;
        private readonly IProverStore<Instrument> _instrumentStore;

        public CertificateService(IDbContextScopeFactory dbContextScopeFactory,
            IProverStore<Certificate> certificateStore, IProverStore<Instrument> instrumentStore)
        {
            _dbContextScopeFactory = dbContextScopeFactory;
            _certificateStore = certificateStore;
            _instrumentStore = instrumentStore;
        }

        public async Task<Certificate> GetCertificate(long number)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var cert = await _certificateStore.Query(x => x.Number == number).FirstOrDefaultAsync();

                if (cert == null)
                    throw new ArgumentException(
                        $"Invalid value provided for certificate #: [{number}]. Couldn't find a certificate with this number.");

                cert.Instruments = await GetInstrumentsOnCertificate(cert);

                return cert;
            }
        }

        public async Task<List<Instrument>> GetInstrumentsOnCertificate(Certificate cert)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _instrumentStore.Query(i => i.CertificateId == cert.Id).ToListAsync();
            }
        }

        public async Task<Certificate> GetCertificate(Guid id)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var cert = await _certificateStore.Query(x => x.Id == id).FirstOrDefaultAsync();
                return await GetCertificate(cert.Number);
            }
        }

        public async Task<long> GetNextCertificateNumber()
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var last = await _certificateStore.GetAll()
                    .Select(x => x.Number)
                    .OrderByDescending(x => x)
                    .FirstOrDefaultAsync();

                return last + 1;
            }
        }

        public async Task<Certificate> CreateCertificate(string testedBy, string verificationType,
            List<Instrument> instruments)
        {
            using (var db = _dbContextScopeFactory.Create())
            {
                var client = instruments.First().Client;

                var certificate = new Certificate
                {
                    CreatedDateTime = DateTime.Now,
                    VerificationType = verificationType,
                    Apparatus = SettingsManager.SettingsInstance.TestSettings.MeasurementApparatus,
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

                await _certificateStore.Upsert(certificate);
                await db.SaveChangesAsync();
                return certificate;
            }
        }

        public async Task<List<long>> GetCertificateNumbers(Client client)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _certificateStore.Query(c => c.Client == client)
                    .Select(c => c.Number)
                    .ToListAsync();
            }
        }

        public Task<List<Certificate>> GetAllCertificates(Client client)
        {
            return GetAllCertificates(client, 0, 0);
        }

        public async Task<List<Certificate>> GetAllCertificates(Client client, long fromNumber, long toNumber)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                return await _certificateStore.Query(c =>
                        (c.ClientId.HasValue && client.Id != Guid.Empty && c.ClientId.Value == client.Id)
                        && (fromNumber == 0 || c.Number >= fromNumber)
                        && (toNumber == 0 || c.Number <= toNumber))
                    .OrderBy(i => i.Number)
                    .ToListAsync();
            }
        }

        public IEnumerable<string> GetDistinctTestedBy()
        {
            _dbContextScopeFactory.CreateReadOnly();

            return _certificateStore.GetAll()
                    .Select(c => c.TestedBy)
                    .Distinct();
        }

        public async Task<List<Instrument>> GetInstrumentsWithNoCertificate(Guid? clientId = null, bool showArchived = false)
        {
            using (_dbContextScopeFactory.CreateReadOnly())
            {
                var results = _instrumentStore.Query(x => x.CertificateId == null
                                                          && ((clientId == Guid.Empty && x.ClientId == null) ||
                                                              x.ClientId == clientId)
                                                          && (showArchived == false && x.ArchivedDateTime == null ||
                                                              showArchived))
                    .OrderBy(x => x.TestDateTime);

                return await results.ToListAsync();
            }
        }
    }
}