using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Prover.Application.Interfaces;
using Prover.Application.Services;
using Prover.Domain.EvcVerifications;
using Prover.Modules.UnionGas.DcrWebService;
using Prover.Modules.UnionGas.MasaWebService;
using Prover.Shared.Interfaces;

namespace Prover.Modules.UnionGas.Exporter
{
    /// <summary>
    ///     Defines the <see cref="ExportToMasaManager" />
    /// </summary>
    public class ExportToMasaManager : IExportVerificationTest
    {
        /// <summary>
        ///     Defines the Log
        /// </summary>
        private static readonly ILogger Log;

        /// <summary>
        ///     Defines the _dcrWebService
        /// </summary>
        private readonly DCRWebServiceSoap _dcrWebService;

        /// <summary>
        ///     Defines the _loginService
        /// </summary>
        private readonly ILoginService<EmployeeDTO> _loginService;

        /// <summary>
        ///     Defines the _testRunService
        /// </summary>
        private readonly EvcVerificationTestService _testRunService;

        public ExportToMasaManager(EvcVerificationTestService testRunService, ILoginService<EmployeeDTO> loginService,
            DCRWebServiceSoap dcrWebService)
        {
            _testRunService = testRunService;
            _dcrWebService = dcrWebService;
            _loginService = loginService;
        }

        public async Task<bool> Export(IEnumerable<EvcVerificationTest> testsForExport)
        {
            var forExport = testsForExport as EvcVerificationTest[] ?? testsForExport.ToArray();
            var qaTestRuns = forExport.Select(Translate.RunTranslationForExport).ToList();

            var isSuccess = await _dcrWebService.SendQaTestResults(qaTestRuns, Log);

            if (!isSuccess)
                throw new Exception(
                    "An error occured sending test results to web service. Please see log for details.");

            foreach (var instr in forExport)
            {
                instr.ExportedDateTime = DateTime.Now;
                await _testRunService.AddOrUpdateVerificationTest(instr);
            }

            return true;
        }

        public Task<bool> Export(EvcVerificationTest verificationTest)
        {
            var instrumentList = new List<EvcVerificationTest> {verificationTest};
            return Export(instrumentList);
        }

        public async Task<bool> ExportFailedTest(string companyNumber)
        {
            var meterDto = await _dcrWebService.FindMeterByInventoryNumber(companyNumber, Log);

            if (meterDto == null)
                throw new Exception($"Inventory #{companyNumber} was not be found on an open job.");

            var failedTest = Translate.CreateFailedTestForExport(meterDto, _loginService.User?.Id);
            return await _dcrWebService.SendQaTestResults(new[] {failedTest});
        }
    }
}