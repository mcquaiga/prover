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
    using System.ServiceModel;
    using System.Threading.Tasks;
    using System.Windows;
    using UnionGas.MASA.DCRWebService;
    using UnionGas.MASA.Validators.CompanyNumber;

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
        /// Defines the _companyNumberValidator
        /// </summary>
        private readonly CompanyNumberValidationManager _companyNumberValidator;

        /// <summary>
        /// Defines the _dcrWebService
        /// </summary>
        private readonly DCRWebServiceSoap _dcrWebService;

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
        /// <param name="companyNumberValidator">The companyNumberValidator<see cref="CompanyNumberValidationManager"/></param>
        /// <param name="loginService">The loginService<see cref="ILoginService{EmployeeDTO}"/></param>
        /// <param name="dcrWebService">The dcrWebService<see cref="DCRWebServiceSoap"/></param>
        public ExportToMasaManager(TestRunService testRunService, CompanyNumberValidationManager companyNumberValidator,
            ILoginService<EmployeeDTO> loginService, DCRWebServiceSoap dcrWebService)
        {
            _testRunService = testRunService;
            _dcrWebService = dcrWebService;
            _companyNumberValidator = companyNumberValidator;
            _loginService = loginService;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the _soapClient
        /// </summary>
        private DCRWebServiceSoapClient _soapClient => (DCRWebServiceSoapClient)_dcrWebService;

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

            var isSuccess = await SendResultsToWebService(qaTestRuns).ConfigureAwait(false);

            if (!isSuccess)
                throw new Exception(
                    "An error occured sending test results to web service. Please see log for details.");

            foreach (var instr in forExport)
            {
                instr.ExportedDateTime = DateTime.Now;
                await _testRunService.Save(instr).ConfigureAwait(false);
            }

            return true;
        }

        /// <summary>
        /// The Export
        /// </summary>
        /// <param name="instrumentForExport">The instrumentForExport<see cref="Instrument"/></param>
        /// <returns>The <see cref="Task{bool}"/></returns>
        public async Task<bool> Export(Instrument instrumentForExport)
        {
            var instrumentList = new List<Instrument> { instrumentForExport };
            return await Export(instrumentList).ConfigureAwait(false);
        }

        /// <summary>
        /// The ExportFailedTest
        /// </summary>
        /// <param name="companyNumber">The companyNumber<see cref="string"/></param>
        /// <returns>The <see cref="Task{bool}"/></returns>
        public async Task<bool> ExportFailedTest(string companyNumber)
        {
            var meterDto = await _companyNumberValidator.ValidateInstrumentExistsOnOpenJob(companyNumber).ConfigureAwait(false);

            if (meterDto == null)
                throw new Exception($"Inventory #{companyNumber} was not be found on an open job.");

            var failedTest = Translate.CreateFailedTestForExport(meterDto, _loginService.User.Id);
            return await SendResultsToWebService(new[] { failedTest }).ConfigureAwait(false);
        }

        /// <summary>
        /// The SendResultsToWebService
        /// </summary>
        /// <param name="evcQaRuns">The evcQaRuns<see cref="IEnumerable{QARunEvcTestResult}"/></param>
        /// <returns>The <see cref="Task{bool}"/></returns>
        private async Task<bool> SendResultsToWebService(IEnumerable<QARunEvcTestResult> evcQaRuns)
        {
            try
            {
                var items = evcQaRuns.ToArray();

                if (!items.Any()) throw new ArgumentOutOfRangeException(nameof(evcQaRuns));

                var request =
                    new SubmitQAEvcTestResultsRequest(new SubmitQAEvcTestResultsRequestBody(items));

                var result = await _dcrWebService.SubmitQAEvcTestResultsAsync(request).ConfigureAwait(false);

                var success = result.Body.SubmitQAEvcTestResultsResult;
                Log.Info($"Web service returned: {success}");

                return string.Equals(success, "success", StringComparison.OrdinalIgnoreCase);
            }
            catch (EndpointNotFoundException ex)
            {
                MessageBox.Show($"MASA Web service could not be reached. Check network connection. {Environment.NewLine}" +
                    $" Endpoint: {_soapClient.Endpoint.Address}");
                Log.Error(ex, $"MASA Web service could not be reached. {Environment.NewLine}" +
                    $" Endpoint: {_soapClient.Endpoint.Address} {Environment.NewLine}" +
                    $" State: {_soapClient.State.ToString()}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An error occured sending results to the web service. {ex}");
            }

            return false;
        }

        #endregion
    }
}
