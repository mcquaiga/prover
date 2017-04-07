using System;
using System.Threading.Tasks;
using Prover.Domain.Instrument.Items;

namespace Prover.CommProtocol.MiHoneywell.Domain.Instrument
{
    internal class HoneywellReadOnlyInstrument : HoneywellInstrument
    {
        public HoneywellReadOnlyInstrument(int id, int accessCode, string name, string itemFilePath)
            : base(id, accessCode, name, itemFilePath)
        {
        }

        public override Task<IPressureItems> GetPressureItems()
        {
            throw new NotSupportedException("Instrument is in read only mode.");
        }

        public override Task<ITemperatureItems> GetTemperatureItems()
        {
            throw new NotSupportedException("Instrument is in read only mode.");
        }

        public override Task<IVolumeItems> GetVolumeItems()
        {
            throw new NotSupportedException("Instrument is in read only mode.");
        }
    }
}