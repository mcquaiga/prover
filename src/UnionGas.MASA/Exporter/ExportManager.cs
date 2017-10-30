using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using NLog;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Instruments;
using Prover.Core.Services;
using Prover.Core.Storage;
using UnionGas.MASA.DCRWebService;

namespace UnionGas.MASA.Exporter
{
    public class ExportToMasaManager : IExportTestRun
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly DCRWebServiceSoap _dcrWebService;
        private readonly TestRunService _testRunService;

        public ExportToMasaManager(TestRunService testRunService, DCRWebServiceSoap dcrWebService)
        {
            _testRunService = testRunService;
            _dcrWebService = dcrWebService;
        }

        public async Task<bool> Export(Instrument instrumentForExport)
        {
            var instrumentList = new List<Instrument> {instrumentForExport};
            return await Export(instrumentList);
        }

        public async Task<bool> Export(IEnumerable<Instrument> instrumentsForExport)
        {
            try
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
                    await _testRunService.Save(instr);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<bool> SendResultsToWebService(IEnumerable<QARunEvcTestResult> evcQaRuns)
        {
            try
            {
                var request =
                    new SubmitQAEvcTestResultsRequest(new SubmitQAEvcTestResultsRequestBody(evcQaRuns.ToArray()));

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