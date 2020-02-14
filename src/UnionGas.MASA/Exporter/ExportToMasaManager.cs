namespace UnionGas.MASA.Exporter
{
    using NLog;
    using Prover.Core.ExternalIntegrations;
    using Prover.Core.Login;
    using Prover.Core.Models.Instruments;
    using Prover.Core.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UnionGas.MASA.DCRWebService;

    /// <summary>
    /// Defines the <see cref="ExportToMasaManager" />
    /// </summary>
    public class ExportToMasaManager : IExportTestRun
    {
        #region Fields

        /// <summary>
        /// Defines the Log
        /// </summary>
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

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
        private readonly TestRunService _testRunService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportToMasaManager"/> class.
        /// </summary>
        /// <param name="testRunService">The testRunService<see cref="TestRunService"/></param>
        /// <param name="loginService">The loginService<see cref="ILoginService{EmployeeDTO}"/></param>
        /// <param name="dcrWebService">The dcrWebService<see cref="DCRWebServiceCommunicator"/></param>
        public ExportToMasaManager(TestRunService testRunService, ILoginService<EmployeeDTO> loginService, DCRWebServiceCommunicator dcrWebService)
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
        /// <returns>The <see cref="Task{bool}"/></returns>
        public async Task<bool> Export(IEnumerable<Instrument> instrumentsForExport)
        {
            var forExport = instrumentsForExport as Instrument[] ?? instrumentsForExport.ToArray();
            var qaTestRuns = forExport.Select(Translate.RunTranslationForExport).ToList();

            var isSuccess = await _dcrWebService.SendQaTestResults(qaTestRuns);

            if (!isSuccess)
                throw new Exception(
                    "An error occured sending test results to web service. Please see log for details.");

            foreach (var instr in forExport)
            {
                instr.ExportedDateTime = DateTime.Now;
                await _testRunService.Save(instr);
            }

            return true;
        }

        /// <summary>
        /// The Export
        /// </summary>
        /// <param name="instrumentForExport">The instrumentForExport<see cref="Instrument"/></param>
        /// <returns>The <see cref="Task{bool}"/></returns>
        public Task<bool> Export(Instrument instrumentForExport)
        {
            var instrumentList = new List<Instrument> { instrumentForExport };
            return Export(instrumentList);
        }

        /// <summary>
        /// The ExportFailedTest
        /// </summary>
        /// <param name="companyNumber">The companyNumber<see cref="string"/></param>
        /// <returns>The <see cref="Task{bool}"/></returns>
        public async Task<bool> ExportFailedTest(string companyNumber)
        {
            Log.Info($"Exporting failed test to MASA. Inventory # {companyNumber}");
            try
            {
                var meterDto = await _dcrWebService.FindMeterByCompanyNumber(companyNumber);

                if (meterDto == null)
                    throw new Exception($"Inventory #{companyNumber} could not be found on an open job.");

                var failedTest = Translate.CreateFailedTestForExport(meterDto, _loginService?.User?.Id);

                return await _dcrWebService.SendQaTestResults(new[] {failedTest});
            }
            catch(Exception ex)
            {
                Log.Error($"Error exporting failed MASA QA test run. {Environment.NewLine}" +
                          $"Exception: {ex}");
            }

            return false;
        }

        #endregion
    }
}
