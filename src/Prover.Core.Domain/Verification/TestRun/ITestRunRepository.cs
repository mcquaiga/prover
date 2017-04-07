using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Shared.Domain;

namespace Prover.Domain.Verification.TestRun
{
    public interface ITestRunRepository : IRepository<TestRun, Guid>
    {
    }
}
