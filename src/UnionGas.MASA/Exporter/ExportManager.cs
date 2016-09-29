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
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly IUnityContainer _container;

        public Uri ServiceUri { get; }

        public ExportManager(IUnityContainer container, string serviceUriString)
        {
            _container = container;
            ServiceUri = new Uri(serviceUriString);
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
            if (isSuccess)
            {
                using (var store = new InstrumentStore(_container))
                {
                    foreach(var instr in forExport)
                    {
                        instr.ExportedDateTime = DateTime.Now;
                        await store.UpsertAsync(instr);
                    }                    
                }
                return true;            
            }
            else
            {
                throw new Exception("An error occured sending test results to web service. Please see log for details.");
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
