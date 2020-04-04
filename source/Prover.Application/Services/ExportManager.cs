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
        private readonly IVerificationTestService _testRunService;

        public ExportManager(IVerificationTestService testRunService)
        {
            _testRunService = testRunService;
        }

        public async Task<bool> Export(EvcVerificationTest verificationTest)
        {
            verificationTest.ExportedDateTime = DateTime.Now;
            await _testRunService.AddOrUpdate(verificationTest);
            return true;
        }

        public async Task<bool> Export(IEnumerable<EvcVerificationTest> verificationTests)
        {
            foreach (var test in verificationTests)
            {
                test.ExportedDateTime = DateTime.Now;
                await _testRunService.AddOrUpdate(test);
            }
            return true;
        }
    }
}
