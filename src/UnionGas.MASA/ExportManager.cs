using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IUnityContainer _container;

        public Uri ServiceUri { get; }

        public ExportManager(IUnityContainer container, string serviceUriString)
        {
            _container = container;
            ServiceUri = new Uri(serviceUriString);
        }

        public async Task<bool> Export(Instrument instrumentForExport)
        {
            var instrumentList = new List<Instrument>();
            instrumentList.Add(instrumentForExport);
            return await Export(instrumentList);
        }

        public async Task<bool> Export(IEnumerable<Instrument> instrumentsForExport)
        {
            var qaTestRuns = new List<DCRWebService.QARunEvcTestResult>();

            foreach (var testRun in instrumentsForExport)
            {
                qaTestRuns.Add(Translate.RunTranslationForExport(testRun));
            }
            
            var isSuccess = await SendResultsToWebService(qaTestRuns);
            if (isSuccess)
            {
                using (var store = new InstrumentStore(_container))
                {
                    foreach(var instr in instrumentsForExport)
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
                var service = new DCRWebService.DCRWebServiceSoapClient("configName", ServiceUri.ToString());
                var result = await service.SubmitQAEvcTestResultsAsync(evcQARun.ToArray());

                if (result.Body.SubmitQAEvcTestResultsResult.ToLower() == "success")
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex, "An error occured sending results to the web service.");
            }

            return false;
        }
    }
}
