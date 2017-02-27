using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;

namespace Prover.Modules.Certificates.Storage
{
    //public class InstrumentStore : ICertificateStore<Instrument>
    //{
    //    private readonly CertificateContext _context;

    //    public InstrumentStore(CertificateContext context)
    //    {
    //        _context = context;
    //    }

    //    public IQueryable<Instrument> Query()
    //    {
    //        return _context.Instruments
    //            .Include(v => v.VerificationTests.Select(t => t.TemperatureTest))
    //            .Include(v => v.VerificationTests.Select(p => p.PressureTest))
    //            .Include(v => v.VerificationTests.Select(vo => vo.VolumeTest))
    //            .AsQueryable();
    //    }

    //    public Instrument Get(Guid id)
    //    {
    //        return Query().FirstOrDefault(x => x.Id == id);
    //    }

    //    public async Task<Instrument> UpsertAsync(Instrument instrument)
    //    {
    //        if (Get(instrument.Id) != null)
    //        {
    //            _context.Instruments.Attach(instrument);
    //            _context.Entry(instrument).State = EntityState.Modified;
    //        }
    //        else
    //        {
    //            _context.Instruments.Attach(instrument);
    //            _context.Entry(instrument).State = EntityState.Added;
    //        }

    //        await _context.SaveChangesAsync();
    //        return instrument;
    //    }

    //    public async Task Delete(Instrument entity)
    //    {
    //        entity.ArchivedDateTime = DateTime.UtcNow;
    //        await UpsertAsync(entity);
    //    }

    //    public void Dispose()
    //    {
    //    }
    //}
}
