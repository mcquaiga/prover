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
    /// Defines the <see cref="ExportToMasaManager" />
    /// </summary>
    public class ExportToMasaManager : IExportVerificationTest
    {
        #region Fields

        /// <summary>
        /// Defines the Log
        /// </summary>
        private static readonly ILogger Log;

        /// <summary>
        /// Defines the _dcrWebService
        /// </summary>
        private readonly DCRWebServiceCommunicator _dcrWebService;

        /// <summary>
        /// Defines the _loginService
        /// </summary>
        private readonly ILoginService<EmployeeDTO> _loginService;

        /// <summary>
        /// Defines the _testRunService
        /// </summary>
        private readonly EvcVerificationTestService _testRunService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportToMasaManager"/> class.
        /// </summary>
        /// <param name="testRunService">The testRunService<see cref="TestRunService"/></param>
        /// <param name="loginService">The loginService<see cref="ILoginService{T}"/></param>
        /// <param name="dcrWebService">The dcrWebService<see cref="DCRWebServiceCommunicator"/></param>
        public ExportToMasaManager(EvcVerificationTestService testRunService, ILoginService<EmployeeDTO> loginService, DCRWebServiceCommunicator dcrWebService)
        {
            _testRunService = testRunService;
            _dcrWebService = dcrWebService;
            _loginService = loginService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The Export
        /// </summary>
        /// <param name="instrumentsForExport">The instrumentsForExport<see cref="IEnumerable{Instrument}"/></param>
        /// <returns>The <see cref="Task"/></returns>
        public async Task<bool> Export(IEnumerable<EvcVerificationTest> testsForExport)
        {
            //var forExport = testsForExport as EvcVerificationTest[] ?? testsForExport.ToArray();
            //var qaTestRuns = forExport.Select(Translate.RunTranslationForExport).ToList();

            //var isSuccess = await _dcrWebService.SendQaTestResults(qaTestRuns);

            //if (!isSuccess)
            //    throw new Exception(
            //        "An error occured sending test results to web service. Please see log for details.");

            //foreach (var instr in forExport)
            //{
            //    instr.ExportedDateTime = DateTime.Now;
            //    await _testRunService.Save(instr);
            //}

            return true;
        }

        /// <summary>
        /// The Export
        /// </summary>
        /// <param name="instrumentForExport">The instrumentForExport<see cref="Instrument"/></param>
        /// <returns>The <see cref="Task{bool}"/></returns>
        public Task<bool> Export(EvcVerificationTest instrumentForExport)
        {
            var instrumentList = new List<EvcVerificationTest> { instrumentForExport };
            return Export(instrumentList);
        }

        /// <summary>
        /// The ExportFailedTest
        /// </summary>
        /// <param name="companyNumber">The companyNumber<see cref="string"/></param>
        /// <returns>The <see cref="Task{bool}"/></returns>
        public async Task<bool> ExportFailedTest(string companyNumber)
        {
            return false;
            //var meterDto = await _dcrWebService.FindMeterByCompanyNumber(companyNumber);

            //if (meterDto == null)
            //    throw new Exception($"Inventory #{companyNumber} was not be found on an open job.");

            //var failedTest = Translate.CreateFailedTestForExport(meterDto, _loginService.User.Id);
            //return await _dcrWebService.SendQaTestResults(new[] { failedTest });
        }

        #endregion
    }
}
