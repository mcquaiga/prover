namespace Prover.Core.VerificationTests.TestActions.PreTestActions
{
    using Prover.CommProtocol.Common;
    using Prover.Core.Models.Instruments;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Subjects;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="ItemUpdaterAction" />
    /// </summary>
    public class ItemUpdaterAction : IInstrumentAction
    {
        #region Fields

        /// <summary>
        /// Defines the _itemsForUpdate
        /// </summary>
        private readonly Dictionary<int, string> _itemsForUpdate;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemUpdaterAction"/> class.
        /// </summary>
        /// <param name="itemsForUpdate">The itemsForUpdate<see cref="Dictionary{int, string}"/></param>
        public ItemUpdaterAction(Dictionary<int, string> itemsForUpdate)
        {
            if (itemsForUpdate == null || !itemsForUpdate.Any())
                throw new ArgumentNullException(nameof(itemsForUpdate));

            _itemsForUpdate = itemsForUpdate;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The Execute
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="instrument">The instrument<see cref="Instrument"/></param>
        /// <param name="statusUpdates">The statusUpdates<see cref="Subject{string}"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public virtual async Task Execute(EvcCommunicationClient commClient, Instrument instrument, Subject<string> statusUpdates = null)
        {
            foreach (var item in _itemsForUpdate)
            {
                await commClient.SetItemValue(item.Key, item.Value);
            }
        }

        #endregion
    }
}
