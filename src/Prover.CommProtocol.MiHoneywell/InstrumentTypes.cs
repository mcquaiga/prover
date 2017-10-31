using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.MiHoneywell.Items;

namespace Prover.CommProtocol.MiHoneywell
{
    public static class HoneywellInstrumentTypes
    {
        public static InstrumentType GetByName(string name)
        {
            var all = GetAll();      

            return all.ToList().FirstOrDefault(i => i.Name == name);
        }

        public static InstrumentType GetById(int id)
        {
            var all = GetAll();

            return all.ToList().FirstOrDefault(i => i.Id == id);
        }

        public static IEnumerable<InstrumentType> GetAll()
        {
            var allTask = ItemHelpers.LoadInstruments();
            allTask.Wait();
            return allTask.Result
                .ToList()
                .OrderBy(i => i.Name);
        }
    }
}