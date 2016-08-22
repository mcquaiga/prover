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
using UnionGas.MASA.Models;

namespace UnionGas.MASA
{
    public static class ExportManager
    {
        private static readonly Uri ServiceAddress = new Uri(@"http://masa");

        public static async Task<bool> Export(IEnumerable<Instrument> instrumentsToExport)
        {
            foreach (var instrument in instrumentsToExport)
            {
                var qaRun = Translate.RunTranslationForExport(instrument);

                var isSuccess = await SendExportDefinition(qaRun);
                
            }

            return true;
        }

        private static async Task<bool> SendExportDefinition(EvcQARun evcQARun)
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
                var response = await client.PostAsync(ServiceAddress, new StringContent(postBody, Encoding.UTF8, "application/xml"));

                return response.IsSuccessStatusCode;
            }
        }
    }
}
