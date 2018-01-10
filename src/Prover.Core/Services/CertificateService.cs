using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Settings;
using Prover.Core.Shared.Data;

namespace Prover.Core.Services
{
    public interface ICertificateService
    {
        Certificate GetCertificate(Guid id);
        Certificate GetCertificate(long number);

        IEnumerable<Instrument> GetInstrumentsWithNoCertificate(Guid? clientId = null, bool showArchived = false);

        long GetNextCertificateNumber();
        Task<Certificate> CreateCertificate(long number, string testedBy, string verificationType, CertificateSettings.MeasurementApparatus measurementApparatus, List<Instrument> instruments);

        IEnumerable<Certificate> GetAllCertificates();
        IEnumerable<Certificate> GetAllCertificates(Client client, long fromNumber = 0, long toNumber = 0);

        IEnumerable<long> GetCertificateNumbers(Client client);
        IEnumerable<string> GetDistinctTestedBy();
    }

    public class CertificateService : ICertificateService
    {
        private readonly IProverStore<Certificate> _certificateStore;
        private readonly IProverStore<Instrument> _instrumentStore;
        private readonly ISettingsService _settingsService;

        public CertificateService(IProverStore<Certificate> certificateStore, IProverStore<Instrument> instrumentStore, ISettingsService settingsService)
        {
            _certificateStore = certificateStore;
            _instrumentStore = instrumentStore;
            _settingsService = settingsService;
        }

        public Certificate GetCertificate(long number)
        {
            var cert = _certificateStore
                .Query(x => x.Number == number)
                .FirstOrDefault();

            if (cert == null) return null;

            cert.Instruments = _instrumentStore.Query(i => i.CertificateId == cert.Id).ToList();

            return cert;
        }

        public Certificate GetCertificate(Guid id)
        {
            return _certificateStore.Query(x => x.Id == id).FirstOrDefault();
        }

        public long GetNextCertificateNumber()
        {
            var qry = _certificateStore.GetAll();
            var last = (long) 0;
            if (qry.Any())
            {
                last = qry.Max(certificate => certificate.Number);
            }

            return last + 1;
        }

        public async Task<Certificate> CreateCertificate(long number, string testedBy, string verificationType, CertificateSettings.MeasurementApparatus measurementApparatus, List<Instrument> instruments)
        {
            var client = instruments.First().Client;
            var certificate = new Certificate
            {
                CreatedDateTime = DateTime.Now,
                VerificationType = verificationType,
                Apparatus = measurementApparatus.SerialNumbers,
                TestedBy = testedBy,
                Client = client,
                ClientId = client.Id,
                Number =  number
            };

            instruments.ForEach(i =>
            {
                i.CertificateId = certificate.Id;
                i.Certificate = certificate;
                certificate.Instruments.Add(i);
            });

            await _certificateStore.Upsert(certificate);
            
            return certificate;
        }

        public IEnumerable<long> GetCertificateNumbers(Client client)
        {
            return _certificateStore.Query(c => c.Client == client)
                .Select(c => c.Number);
        }

        public IEnumerable<Certificate> GetAllCertificates()
        {
            return _certificateStore.GetAll().ToList();
        }

        public IEnumerable<Certificate> GetAllCertificates(Client client, long fromNumber = 0, long toNumber = 0)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            return _certificateStore.Query(c => c.ClientId.HasValue && client.Id != Guid.Empty && c.ClientId.Value == client.Id
                                                && (fromNumber == 0 || c.Number >= fromNumber) 
                                                && (toNumber == 0 || c.Number <= toNumber))
                .OrderBy(i => i.Number);
        }

        public IEnumerable<string> GetDistinctTestedBy()
        {
            return _certificateStore.GetAll()                
                .Select(c => c.TestedBy)
                .Distinct();
        }

        public IEnumerable<Instrument> GetInstrumentsWithNoCertificate(Guid? clientId = null, bool showArchived = false)
        {
            try
            {
                if (clientId == Guid.Empty)
                    clientId = null;

                var results = _instrumentStore.Query(x => 
                        x.CertificateId == null 
                    &&  x.ClientId == clientId
                    &&  x.ArchivedDateTime == null);
                
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