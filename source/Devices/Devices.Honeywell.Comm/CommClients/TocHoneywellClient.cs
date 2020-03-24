//namespace Devices.Honeywell.Comm.CommClients
//{
//    using Devices.Communications.IO;
//    using Devices.Core.Interfaces.Items;
//    using Devices.Core.Items;
//    using Devices.Honeywell.Core;
//    using System.Collections.Generic;
//    using System.Threading;
//    using System.Threading.Tasks;

// ///
// <summary>
// /// Defines the <see cref="TocHoneywellClient"/> ///
// </summary>
// public sealed class TocHoneywellClient : HoneywellClient { #region Constructors

// public TocHoneywellClient(ICommPort commPort, HoneywellEvcType evcType) : base(commPort, evcType)
// { _tibBoardItems = HoneywellDeviceTypes.TibBoard.Definitions; }

// #endregion

// #region Fields

// /// <summary> /// Defines the _tibBoardItems /// </summary> private readonly
// IEnumerable<ItemMetadata> _tibBoardItems;

// #endregion

// #region Methods

// /// <summary> /// The GetFrequencyItems /// </summary> /// <returns>The <see
// cref="Task{IFrequencyTestItems}"/></returns> public override async Task<IFrequencyTestItems>
// GetFrequencyItems() { var mainResults = await GetItemValues(ItemDetails.FrequencyTestItems());
// await Disconnect();

// try { EvcDeviceType = HoneywellDeviceTypes.TibBoard; await Connect(new CancellationToken()); var
// tibResults = await GetItemValues(_tibBoardItems.FrequencyTestItems());

// return new FrequencyTestItems(mainResults, tibResults); } finally { await Disconnect();
// EvcDeviceType = HoneywellDeviceTypes.Toc; } }

// /// <summary> /// The GetItemValues /// </summary> /// <param name="itemNumbers">The itemNumbers
// <see cref="IEnumerable{ItemMetadata}"/></param> /// <returns>The <see
// cref="Task{IEnumerable{ItemValue}}"/></returns> public override async Task<IEnumerable<ItemValue>>
// GetItemValues(IEnumerable<ItemMetadata> itemNumbers) { // The Tib Boards don't support the
// ReadGroup command, so we need to read one item at a time if (EvcDeviceType ==
// HoneywellDeviceTypes.TibBoard) { var results = new List<ItemValue>();

// foreach (var item in itemNumbers) { results.Add(await GetItemValue(item)); }

// return results; }

// return await base.GetItemValues(itemNumbers); }

//        #endregion
//    }
//}