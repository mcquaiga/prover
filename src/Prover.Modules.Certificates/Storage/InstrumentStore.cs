using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.Modules.Certificates.Models;

namespace Prover.Modules.Certificates.Storage
{
    public class CertificateInstrumentStore : ICertificateStore<CertificateInstrument>
    {
        private readonly CertificateContext _context;

        public CertificateInstrumentStore(CertificateContext context)
        {
            _context = context;
        }

        public IQueryable<CertificateInstrument> Query()
        {
            return _context.CertificateInstruments
                .Include(v => v.VerificationTests.Select(t => t.TemperatureTest))
                .Include(v => v.VerificationTests.Select(p => p.PressureTest))
                .Include(v => v.VerificationTests.Select(vo => vo.VolumeTest))
                .AsQueryable();
        }

        public CertificateInstrument Get(Guid id)
        {
            return Query().FirstOrDefault(x => x.Id == id);
        }

        public async Task<CertificateInstrument> UpsertAsync(CertificateInstrument instrument)
        {
            if (Get(instrument.Id) != null)
            {
                _context.CertificateInstruments.Attach(instrument);
                _context.Entry(instrument).State = EntityState.Modified;
            }
            else
            {
                _context.CertificateInstruments.Attach(instrument);
                _context.Entry(instrument).State = EntityState.Added;
            }

            await _context.SaveChangesAsync();
            return instrument;
        }

        public async Task Delete(CertificateInstrument entity)
        {
            entity.ArchivedDateTime = DateTime.UtcNow;
            await UpsertAsync(entity);
        }

        public void Dispose()
        {
        }
    }
}
