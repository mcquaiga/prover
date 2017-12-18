using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell.Items;

namespace Prover.CommProtocol.MiHoneywell.CommClients
{
    public sealed class TocHoneywellClient : HoneywellClient
    {
        internal static InstrumentType TurboMonitor = new InstrumentType()
        {
            Id = 6,
            AccessCode = 6,
            Name = "Turbo Monitor",
            ItemsMetadata = new List<ItemMetadata>()
            {
                new ItemMetadata() { Code = "HIGHRES", Number = 851, IsFrequencyTest = true}
            }
        };

        public TocHoneywellClient(CommPort commPort, InstrumentType instrumentType, ISubject<string> statusSubject) : base(commPort, instrumentType, statusSubject)
        {
        }

        public override async Task<IFrequencyTestItems> GetFrequencyItems()
        {
            var firstInstrument = InstrumentType;
            var results = await GetItemValues(InstrumentType.ItemsMetadata.FrequencyTestItems());
            await Disconnect();
            Thread.Sleep(1000);

            InstrumentType = TurboMonitor;
            await Connect(new CancellationToken());
            var tibResults = await GetItemValues(InstrumentType.ItemsMetadata.FrequencyTestItems());
            await Disconnect();

            Thread.Sleep(1000);

            InstrumentType = firstInstrument;

            var values = results.ToList();
            values.AddRange(tibResults.ToList());
            return new FrequencyTestItems(values);
        }
    }
}