using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.CommProtocol.Common.Messaging
{
    public abstract class ResponseProcessor<T>
    {
        public abstract IObservable<T> ResponseObservable(IObservable<char> source);
    }
}
