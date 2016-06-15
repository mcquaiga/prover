using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.CommProtocol.Common.Messaging
{
    public abstract class ResponseCode
    {
        protected ResponseCode(int code)
        {
            Code = code;
        }

        public int Code { get; }
    }
}
