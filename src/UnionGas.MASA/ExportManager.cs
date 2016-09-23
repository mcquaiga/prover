using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Prover.Core.Models.Instruments;
using NLog;
using UnionGas.MASA.Models;
using Prover.Core.ExternalIntegrations;
using Microsoft.Practices.ObjectBuilder2;
using Prover.Core.Storage;
using Microsoft.Practices.Unity;

namespace UnionGas.MASA
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
            var qaTestRuns = forExport.Select(testRun => Translate.RunTranslationForExport(testRun)).ToList();

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
