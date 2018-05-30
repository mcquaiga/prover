using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using NLog;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Login;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Validators.CompanyNumber;

namespace UnionGas.MASA.Exporter
{
    public class ExportManager : IExportTestRun
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly DCRWebServiceSoap _dcrWebService;
        private readonly IInstrumentStore<Instrument> _instrumentStore;
        private readonly CompanyNumberValidator _companyNumberValidator;
        private readonly ILoginService<EmployeeDTO> _loginService;

        public ExportManager(IInstrumentStore<Instrument> instrumentStore, DCRWebServiceSoap dcrWebService, CompanyNumberValidator companyNumberValidator, ILoginService<EmployeeDTO> loginService)
        {
            _instrumentStore = instrumentStore;
            _dcrWebService = dcrWebService;
            _companyNumberValidator = companyNumberValidator;
            _loginService = loginService;
        }

        public async Task<bool> Export(Instrument instrumentForExport)
        {
            var instrumentList = new List<Instrument> {instrumentForExport};
            return await Export(instrumentList);
        }

        public async Task<bool> Export(IEnumerable<Instrument> instrumentsForExport)
        {          
                var forExport = instrumentsForExport as Instrument[] ?? instrumentsForExport.ToArray();
                var qaTestRuns = forExport.Select(Translate.RunTranslationForExport).ToList();

                var isSuccess = await SendResultsToWebService(qaTestRuns);

                if (!isSuccess)
                    throw new Exception(
                        "An error occured sending test results to web service. Please see log for details.");

                foreach (var instr in forExport)
                {
                    instr.ExportedDateTime = DateTime.Now;
                    await _instrumentStore.UpsertAsync(instr);
                }

                return true;           
        }

        public async Task<bool> ExportFailedTest(string companyNumber)
        {
            var meterDto = await _companyNumberValidator.VerifyWithWebService(companyNumber);

            if (meterDto != null)
            {
                Log.Info("Company number verified.");
                var failedTest = Translate.CreateFailedTestForExport(meterDto, _loginService.User.EmployeeNbr);
               
                var isSuccess = await SendResultsToWebService(new[] {failedTest});

                return true;
            }
            Log.Error("Company number not recognized.");
            return false;
        }

        private async Task<bool> SendResultsToWebService(IEnumerable<QARunEvcTestResult> evcQaRuns)
        {
            try
            {
                var items = evcQaRuns.ToArray();

                if (!items.Any()) throw new ArgumentOutOfRangeException(nameof(evcQaRuns));

                var request =
                    new SubmitQAEvcTestResultsRequest(new SubmitQAEvcTestResultsRequestBody(items));

                var result = await _dcrWebService.SubmitQAEvcTestResultsAsync(request);

                if (result.Body.SubmitQAEvcTestResultsResult.ToLower() == "success")
                {
                    Log.Info($"Successfully exported instrument(s).");
                    return true;
                }

                Log.Warn($"Web service returned: {result.Body.SubmitQAEvcTestResultsResult}");
            }
            catch (EndpointNotFoundException)
            {
                MessageBox.Show("Web service could not be reached.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An error occured sending results to the web service. {ex}");
            }

            return false;
        }
    }
}