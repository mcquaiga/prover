using System.Reactive.Subjects;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.Core.Models.Instruments;

namespace Prover.Core.VerificationTests.TestActions
{
    public abstract class TestActionBase
    {
        public abstract Task Execute(EvcCommunicationClient commClient, Instrument instrument,
            Subject<string> statusUpdates = null);
    }

    public abstract class PostTestResetBase : TestActionBase
    {
    }

    public abstract class PreTestValidationBase : TestActionBase
    {
    }
}