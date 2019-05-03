using Devices.Communications.Messaging;
using Devices.Honeywell.Comm.Exceptions;

namespace Devices.Honeywell.Comm.Messaging.Responses.Codes
{
    internal static class ResponseCodes
    {
        internal static EvcResponseException GetException(StatusResponseMessage response)
        {
            switch (response.ResponseCode)
            {
                case ResponseCode.SignOnError:
                    return new SignOnErrorException(response);

                default:
                    return new EvcResponseException($"Unknown response code {response.ResponseCode}.");
            }
        }
    }
}