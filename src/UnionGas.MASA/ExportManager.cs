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

namespace UnionGas.MASA
{
    public class ExportManager
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public Uri ServiceUri { get; }

        public ExportManager(string serviceUriString)
        {
            ServiceUri = new Uri(serviceUriString);
        }

        public async Task<bool> Export( IEnumerable<Instrument> instrumentsToExport)
        {
            foreach (var instrument in instrumentsToExport)
            {
                var qaRun = Translate.RunTranslationForExport(instrument);
                var isSuccess = await SendExportDefinition(qaRun);
                if (isSuccess)
                    instrument.ExportedDateTime = DateTime.Now;
            }

            return true;
        }

        private async Task<bool> SendExportDefinition(EvcQARun evcQARun)
        {
            var serializer = new XmlSerializer(typeof(EvcQARun));
            var result = new StringBuilder();
            using (var writer = XmlWriter.Create(result))
            {
                serializer.Serialize(writer, evcQARun);
            }
            var postBody = result.ToString();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

                var response = await client.PostAsync(
                                    ServiceUri, 
                                    new StringContent(postBody, Encoding.UTF8, "application/xml"));
                return response.IsSuccessStatusCode;
            }
        }
    }
}
