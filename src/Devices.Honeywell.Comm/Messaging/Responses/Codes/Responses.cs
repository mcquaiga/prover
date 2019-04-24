using Devices.Honeywell.Comm.CommClients;
using System;
using System.Collections.Generic;

namespace Devices.Honeywell.Comm.Messaging.Responses.Codes
{
    internal static class Responses
    {
        #region Methods

        public static HoneywellResponse Get(ResponseCode code)
        {
            return All.Find(h => h.Code == code);
        }

        #endregion

        #region Properties

        public static List<HoneywellResponse> All
        {
            get
            {
                if (_all == null || _all.Count == 0)
                {
                    BuildList();
                }
                return _all;
            }
        }

        public static HoneywellResponse SignOnError =>
                            new HoneywellResponse(ResponseCode.SignOnError, response => new SignOnErrorException(response));

        private static List<HoneywellResponse> _all;

        private static void BuildList()
        {
            _all = new List<HoneywellResponse>()
            {
                SignOnError,
                new FramingErrorCode()
            };
        }

        #endregion
    }

    internal class FramingErrorCode : HoneywellResponse
    {
        #region Constructors

        public FramingErrorCode()
            : base(ResponseCode.FramingError)
        {
        }

        protected override Action<HoneywellClient> RecoveryAction => client =>
        {
            client.CommPort.Close();
        };

        #endregion
    }
}