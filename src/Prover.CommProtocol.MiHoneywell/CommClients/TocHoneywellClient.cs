namespace Prover.CommProtocol.MiHoneywell.CommClients
{
using System.Reactive.Subjects;
    using Prover.CommProtocol.Common;
    using Prover.CommProtocol.Common.IO;
    using Prover.CommProtocol.Common.Items;
    using Prover.CommProtocol.MiHoneywell.Items;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Prover.CommProtocol.Common.Models.Instrument;

    /// <summary>
    /// Defines the <see cref="TocHoneywellClient" />
    /// </summary>
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

        /// <summary>
        /// Defines the _tibBoardItems
        /// </summary>
        private readonly IEnumerable<ItemMetadata> _tibBoardItems;

        public TocHoneywellClient(ICommPort commPort, IEvcDevice instrumentType, ISubject<string> statusSubject) : base(commPort, instrumentType, statusSubject)
        {
            _tibBoardItems = HoneywellInstrumentTypes.TurboMonitor.ItemsMetadata;
        }


        #region Methods

        /// <summary>
        /// The GetFrequencyItems
        /// </summary>
        /// <returns>The <see cref="Task{IFrequencyTestItems}"/></returns>
        public override async Task<IFrequencyTestItems> GetFrequencyItems()
        {
            var mainResults = await GetItemValues(ItemDetails.FrequencyTestItems());
            await Disconnect();
            

            try
            {
                InstrumentType = HoneywellInstrumentTypes.TurboMonitor;
                await Connect(new CancellationToken());
                var tibResults = await GetItemValues(_tibBoardItems.FrequencyTestItems());

                return new FrequencyTestItems(mainResults, tibResults);
            }
            finally
            {
                await Disconnect();              
                InstrumentType = HoneywellInstrumentTypes.Toc;
            }
        }

        /// <summary>
        /// The GetItemValues
        /// </summary>
        /// <param name="itemNumbers">The itemNumbers<see cref="IEnumerable{ItemMetadata}"/></param>
        /// <returns>The <see cref="Task{IEnumerable{ItemValue}}"/></returns>
        public override async Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<ItemMetadata> itemNumbers)
        {
            if (InstrumentType == HoneywellInstrumentTypes.TurboMonitor)
            {
                var results = new List<ItemValue>();

                foreach (var item in itemNumbers)
                {
                    results.Add(await GetItemValue(item));
                }

                return results;
            }

            return await base.GetItemValues(itemNumbers);
        }

        #endregion
    }
}
