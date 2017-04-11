using System.Threading.Tasks;
using Prover.Core.DomainPortable.Instrument;

namespace Prover.Domain.Verification.TestRun
{
    public class TestRunFactory
    {
        public static async Task<TestRun> Create(IInstrumentFactory instrumentFactory)
        {
            var instrument = await instrumentFactory.Create();

            var testRun = new TestRun(instrument);

            return testRun;
        }
    }
}