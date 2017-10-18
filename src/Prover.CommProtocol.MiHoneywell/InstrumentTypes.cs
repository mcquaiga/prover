using System.Collections.Generic;
using System.Linq;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.MiHoneywell.Items;

namespace Prover.CommProtocol.MiHoneywell
{
    public static class HoneywellInstrumentTypes
    {
        public static InstrumentType GetByName(string name)
        {
            return GetAll().FirstOrDefault(
                i => i.Name == name);
        }

        public static IEnumerable<InstrumentType> GetAll()
        {
            return ItemHelpers.LoadInstruments()
                .OrderBy(i => i.Name);
        }
    }
}