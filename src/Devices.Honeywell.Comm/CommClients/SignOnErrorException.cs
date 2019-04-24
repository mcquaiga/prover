using Devices.Communications.Messaging;
using Devices.Honeywell.Comm.Messaging.Responses;
using System;

namespace Devices.Honeywell.Comm.CommClients
{
    [Serializable]
    public class SignOnErrorException : EvcResponseException
    {
        #region Constructors

        public SignOnErrorException(StatusResponseMessage message)
            : base("Sign on error occured.")
        {
        }

        #endregion
    }

    internal static class ResponseCodes
    {
        #region Methods

        internal static EvcResponseException GetException(StatusResponseMessage response)
        {
            switch (response.ResponseCode)
            {
                case ResponseCode.NoData:
                    break;

                case ResponseCode.NoError:
                    break;

                case ResponseCode.MessageFormatError:
                    break;

                case ResponseCode.SignOnError:
                    return new SignOnErrorException(response);

                case ResponseCode.TimeoutError:
                    break;

                case ResponseCode.FramingError:
                    break;

                case ResponseCode.CheckByteError:
                    break;

                case ResponseCode.InvalidAccessCodeError:
                    break;

                case ResponseCode.InvalidCommandError:
                    break;

                case ResponseCode.InvalidItemNumberError:
                    break;

                case ResponseCode.InvalidEnquiryError:
                    break;

                case ResponseCode.TooManyRetransmissionsError:
                    break;

                case ResponseCode.ReadOnlyError:
                    break;

                case ResponseCode.NoAuditTrailError:
                    break;

                case ResponseCode.EventLoggerFullError:
                    break;

                case ResponseCode.InvalidAGA8Condition:
                    break;

                case ResponseCode.InvalidDataError:
                    break;

                case ResponseCode.InvalidChangeAttempted:
                    break;

                case ResponseCode.InsufficientPower:
                    break;

                case ResponseCode.REIError:
                    break;

                default:
                    return new EvcResponseException($"Unknown response code {response.ResponseCode}.");
            }

            return new EvcResponseException($"Unknown response code {response.ResponseCode}.");
        }

        #endregion
    }
}