using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;

namespace Prover.CommProtocol.MiHoneywell.CommClients
{
    public sealed class TocHoneywellClient : HoneywellClient
    {
        public static InstrumentType TurboMonitor = new InstrumentType()
        {
            Id = 6,
            AccessCode = 6,
            Name = "Turbo Monitor",
            ItemFilePath = "TurboMonitorItems.xml"
        };

        private readonly IEnumerable<ItemMetadata> _tibBoardItems;

        public TocHoneywellClient(CommPort commPort, InstrumentType instrumentType) : base(commPort, instrumentType)
        {
            //_tibBoardItems = TurboMonitor;
        }

        public override async Task<IEnumerable<ItemValue>> GetFrequencyItems()
        {
            var results = await base.GetFrequencyItems();
            //await Disconnect();
            //Thread.Sleep(1000);

            //InstrumentType = TurboMonitor;
            //await Connect();
            //var tibResults = await GetItemValues(_tibBoardItems.FrequencyTestItems());
            //await Disconnect();
            //Thread.Sleep(1000);

            //InstrumentType = Instruments.Toc;

            var values = results.ToList();
            //values.AddRange(tibResults.ToList());
            return values;
        }
    }
}