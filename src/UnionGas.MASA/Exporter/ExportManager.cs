using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Unity;
using NLog;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using UnionGas.MASA.DCRWebService;

namespace UnionGas.MASA.Exporter
{
    public class ExportManager : IExportTestRun
    {
        private readonly IInstrumentStore<Instrument> _instrumentStore;
        private readonly DCRWebService.DCRWebServiceSoap _dcrWebService;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public ExportManager(IInstrumentStore<Instrument> instrumentStore, DCRWebService.DCRWebServiceSoap dcrWebService)
        {
            _instrumentStore = instrumentStore;
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
                {
                    throw new Exception("An error occured sending test results to web service. Please see log for details.");
                }

                foreach (var instr in forExport)
                {
                    instr.ExportedDateTime = DateTime.Now;
                    await _instrumentStore.UpsertAsync(instr);
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        private async Task<bool> SendResultsToWebService(IEnumerable<DCRWebService.QARunEvcTestResult> evcQARun)
        {
            try
            {
                var request =
                    new SubmitQAEvcTestResultsRequest(new SubmitQAEvcTestResultsRequestBody(evcQARun.ToArray()));

                var result = await _dcrWebService.SubmitQAEvcTestResultsAsync(request);

                if (result.Body.SubmitQAEvcTestResultsResult.ToLower() == "success")
                {
                    return true;
                }
                
                Log.Warn($"Web service returned: {result.Body.SubmitQAEvcTestResultsResult}");
            }
            catch (Exception ex)
            { 
                Log.Error(ex, "An error occured sending results to the web service.");
            }

            MessageBox.Show($"An error occured sending results to the web service. " +
                            $"{Environment.NewLine} See log for details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            return false;
        }
    }
}
