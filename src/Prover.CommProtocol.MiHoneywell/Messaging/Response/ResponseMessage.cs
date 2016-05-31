using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.CommProtocol.MiHoneywell.Messaging.Response
{
    internal abstract class ResponseMessage
    {
        protected ResponseMessage(string checksum)
        {
            Checksum = checksum;
        }

        public string Checksum { get; protected set; }
    }

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
            return $"{ResponseCode}";
        }
    }
}
