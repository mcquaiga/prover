using Devices.Honeywell.Comm.CommClients;
using Devices.Honeywell.Comm.Exceptions;
using System;
using System.Collections.Generic;

namespace Devices.Honeywell.Comm.Messaging.Responses.Codes
{
    internal static class Responses
    {
        private static List<HoneywellResponse> _all;

        public static HoneywellResponse SignOnError =>
                            new HoneywellResponse(ResponseCode.SignOnError, response => new SignOnErrorException(response));

        private static List<HoneywellResponse> All
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

        public static HoneywellResponse Get(ResponseCode code)
        {
            return All.Find(h => h.Code == code);
        }

        private static void BuildList()
        {
            _all = new List<HoneywellResponse>()
            {
                SignOnError,
                new FramingErrorCode()
            };
        }
    }

    internal class FramingErrorCode : HoneywellResponse
    {
        public FramingErrorCode()
            : base(ResponseCode.FramingError)
        {
        }

        protected override Action<HoneywellClient> RecoveryAction => client =>
        {
            client.CommPort.Close();
        };
    }
}