using Devices.Communications.Messaging;
using System.Collections.Generic;

namespace Devices.Honeywell.Comm.Messaging.Responses
{
    public abstract class BaseResponseMessage : ResponseMessage
    {
        #region Properties

        public string Checksum { get; protected set; }

        #endregion

        #region Constructors

        protected BaseResponseMessage()
        {
        }

        protected BaseResponseMessage(string checksum)
        {
            Checksum = checksum;
        }

        #endregion
    }

    public class StatusResponseMessage : BaseResponseMessage
    {
        #region Constructors

        public StatusResponseMessage(ResponseCode code, string checksum) : base(checksum)
        {
            ResponseCode = code;
        }

        #endregion

        #region Properties

        public bool IsSuccess
            => ResponseCode == ResponseCode.NoError;

        public ResponseCode ResponseCode { get; }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"Code = {ResponseCode}; Checksum = {Checksum}";
        }

        #endregion
    }

    internal class ItemGroupResponseMessage : BaseResponseMessage
    {
        #region Constructors

        public ItemGroupResponseMessage(Dictionary<int, string> itemValues, string checksum) : base(checksum)
        {
            ItemValues = itemValues;
        }

        #endregion

        #region Properties

        public Dictionary<int, string> ItemValues { get; set; }

        #endregion
    }

    internal class ItemValueResponseMessage : BaseResponseMessage
    {
        #region Constructors

        public ItemValueResponseMessage(int itemNumber, string rawValue, string checksum) : base(checksum)
        {
            ItemNumber = itemNumber;
            RawValue = rawValue;
        }

        #endregion

        #region Properties

        public int ItemNumber { get; set; }

        public string RawValue { get; set; }

        #endregion
    }
}