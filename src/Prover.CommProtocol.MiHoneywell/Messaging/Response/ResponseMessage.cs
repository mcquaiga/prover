using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Prover.CommProtocol.Common.Messaging;

namespace Prover.CommProtocol.MiHoneywell.Messaging.Response
{
    internal class StatusResponseMessage : ResponseMessage
    {
        public StatusResponseMessage(ResponseCode code, string checksum) : base(checksum)
        {
            ResponseCode = code;
        }

        public ResponseCode ResponseCode { get; }

        public bool IsSuccess
            => ResponseCode == ResponseCode.NoError || ResponseCode == ResponseCode.InvalidEnquiryError;

        public override string ToString()
        {
            return $"Code = {ResponseCode}; Checksum = {Checksum}";
        }
    }
}
