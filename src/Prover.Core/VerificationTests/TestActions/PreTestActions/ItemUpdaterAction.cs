namespace Prover.Core.VerificationTests.TestActions.PreTestActions
{
    using Prover.CommProtocol.Common;
    using Prover.CommProtocol.Common.Items;
    using Prover.Core.Models.Instruments;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive.Subjects;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="ItemUpdaterAction" />
    /// </summary>
    public class ItemUpdaterAction : IEvcDeviceValidationAction
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
        public ItemUpdaterAction(VerificationStep verificationStep, Dictionary<int, string> itemsForUpdate)
        {
            if (itemsForUpdate == null || !itemsForUpdate.Any())
                throw new ArgumentNullException(nameof(itemsForUpdate));

            _itemsForUpdate = itemsForUpdate;
            VerificationStep = verificationStep;
        }

        public VerificationStep VerificationStep { get; protected set; }

        #endregion

        #region Methods

        /// <summary>
        /// The Execute
        /// </summary>
        /// <param name="commClient">The commClient<see cref="EvcCommunicationClient"/></param>
        /// <param name="instrument">The instrument<see cref="Instrument"/></param>
        /// <param name="statusUpdates">The statusUpdates<see cref="Subject{string}"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public virtual async Task Execute(EvcCommunicationClient commClient, Instrument instrument, 
            CancellationToken ct = new CancellationToken(), Subject<string> statusUpdates = null)
        {
            foreach (var item in _itemsForUpdate)
            {
                if (commClient.InstrumentType.Items.GetItem(item.Key) != null)
                {
                    await commClient.SetItemValue(item.Key, item.Value);

                    if (instrument.Items.GetItem(item.Key) != null)
                        instrument.Items.GetItem(item.Key).RawValue = item.Value;
                }                
            }
        }

        #endregion
    }
}
