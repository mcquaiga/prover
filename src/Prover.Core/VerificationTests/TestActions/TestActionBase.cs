using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.Core.Models.Instruments;

namespace Prover.Core.VerificationTests.TestActions
{
    public abstract class TestActionBase
    {
        public abstract Task Execute(EvcCommunicationClient commClient, Instrument instrument);
    }

    public abstract class PostTestResetBase : TestActionBase
    {
        
    }

    public abstract class PreTestValidationBase : TestActionBase
    {      
    }
}
