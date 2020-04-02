using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Prover.Application.Interfaces;
using Prover.Domain.EvcVerifications;

namespace Prover.Application.Services
{
    public class ExportManager : IExportVerificationTest
    {
        private readonly EvcVerificationTestService _testRunService;

        public ExportManager(EvcVerificationTestService testRunService)
        {
            _testRunService = testRunService;
        }

        public async Task<bool> Export(EvcVerificationTest verificationTest)
        {
            verificationTest.ExportedDateTime = DateTime.Now;
            await _testRunService.AddOrUpdateVerificationTest(verificationTest);
            return true;
        }

        public async Task<bool> Export(IEnumerable<EvcVerificationTest> verificationTests)
        {
            foreach (var test in verificationTests)
            {
                test.ExportedDateTime = DateTime.Now;
                await _testRunService.AddOrUpdateVerificationTest(test);
            }
            return true;
        }
    }
}
