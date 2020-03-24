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
        /// <summary>
        /// Defines the _tibBoardItems
        /// </summary>
        private readonly IEnumerable<ItemMetadata> _tibBoardItems;

        public TocHoneywellClient(ICommPort commPort, IEvcDevice instrumentType, ISubject<string> statusSubject) : base(commPort, instrumentType, statusSubject)
        {
            _tibBoardItems = HoneywellInstrumentTypes.TibBoard.ItemsMetadata;
        }

        #region Methods

        /// <summary>
        /// The GetFrequencyItems
        /// </summary>
        /// <returns>The <see cref="Task{IFrequencyTestItems}"/></returns>
        public override async Task<IFrequencyTestItems> GetFrequencyItems()
        {
            var mainResults = await GetItemValues(InstrumentType.ItemsMetadata.FrequencyTestItems());
            await Disconnect();
            Thread.Sleep(1000);
            try
            {
                InstrumentType = HoneywellInstrumentTypes.TibBoard;
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
            // The Tib Boards don't support the ReadGroup command, so we need to read one item at a time
            if (InstrumentType == HoneywellInstrumentTypes.TibBoard)
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
