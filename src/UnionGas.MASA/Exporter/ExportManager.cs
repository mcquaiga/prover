using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using NLog;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;

namespace UnionGas.MASA.Exporter
{
    public class ExportManager : IExportTestRun
    {
        private readonly IInstrumentStore<Instrument> _instrumentStore;
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public Uri ServiceUri { get; set; }

        public ExportManager(IInstrumentStore<Instrument> instrumentStore)
        {
            _instrumentStore = instrumentStore;
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
                using (var service = new DCRWebService.DCRWebServiceSoapClient())
                {
                    service.Endpoint.Address = new EndpointAddress(ServiceUri);

                    var result = await service.SubmitQAEvcTestResultsAsync(evcQARun.ToArray());
                    if (result.Body.SubmitQAEvcTestResultsResult.ToLower() == "success")
                    {
                        return true;
                    }
                }
                
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occured sending results to the web service.");
            }

            return false;
        }
    }
}
