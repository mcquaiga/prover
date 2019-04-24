using Devices.Communications.Messaging;
using System.Collections.Generic;

namespace Devices.Honeywell.Comm.Messaging.Responses
{
    internal static class ResponseProcessors
    {
        #region Fields

        public static ResponseProcessor<StatusResponseMessage>
            ResponseCode = new ResponseCodeProcessor();

        #endregion

        #region Methods

        public static ResponseProcessor<ItemGroupResponseMessage>
            ItemGroup(IEnumerable<int> itemNumbers) => new ItemGroupValuesProcessor(itemNumbers);

        public static ResponseProcessor<ItemValueResponseMessage>
                    ItemValue(int itemNumber) => new ItemValueProcessor(itemNumber);

        #endregion
    }
}