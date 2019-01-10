namespace Prover.CommProtocol.MiHoneywell.CommClients
{
    using Prover.CommProtocol.Common;
    using Prover.CommProtocol.Common.IO;
    using Prover.CommProtocol.Common.Items;
    using Prover.CommProtocol.MiHoneywell.Items;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="TocHoneywellClient" />
    /// </summary>
    public sealed class TocHoneywellClient : HoneywellClient
    {
        #region Fields

        /// <summary>
        /// Defines the _tibBoardItems
        /// </summary>
        private readonly IEnumerable<ItemMetadata> _tibBoardItems;

        /// <summary>
        /// Defines the TurboMonitor
        /// </summary>
        public static InstrumentType TurboMonitor = new InstrumentType()
        {
            Id = 6,
            AccessCode = 6,
            Name = "Turbo Monitor",
            ItemFilePath = "TurboMonitorItems.xml"
        };

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TocHoneywellClient"/> class.
        /// </summary>
        /// <param name="commPort">The commPort<see cref="CommPort"/></param>
        /// <param name="instrumentType">The instrumentType<see cref="InstrumentType"/></param>
        public TocHoneywellClient(CommPort commPort, InstrumentType instrumentType) : base(commPort, instrumentType)
        {
            _tibBoardItems = ItemHelpers.LoadItems(TurboMonitor);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The GetFrequencyItems
        /// </summary>
        /// <returns>The <see cref="Task{IFrequencyTestItems}"/></returns>
        public override async Task<IFrequencyTestItems> GetFrequencyItems()
        {
            var mainResults = await GetItemValues(ItemDetails.FrequencyTestItems());
            await Disconnect();
            Thread.Sleep(1000);

            try
            {
                InstrumentType = TurboMonitor;
                await Connect();
                var tibResults = await GetItemValues(_tibBoardItems.FrequencyTestItems());

                return new FrequencyTestItems(mainResults, tibResults);
            }
            finally
            {
                await Disconnect();
                Thread.Sleep(500);
                InstrumentType = Instruments.Toc;
            }
        }

        /// <summary>
        /// The GetItemValues
        /// </summary>
        /// <param name="itemNumbers">The itemNumbers<see cref="IEnumerable{ItemMetadata}"/></param>
        /// <returns>The <see cref="Task{IEnumerable{ItemValue}}"/></returns>
        public override async Task<IEnumerable<ItemValue>> GetItemValues(IEnumerable<ItemMetadata> itemNumbers)
        {
            if (InstrumentType == TurboMonitor)
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
