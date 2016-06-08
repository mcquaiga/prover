using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace UnionGas.MASA
{
    public class CompanyNumberVerifier
    {
        private const string VerifyEndPoint = "Verify";
        private const string UpdateEndPoint = "Update";

        public CompanyNumberVerifier(Uri serverUri, long serialNumber, long companyNumber)
        {
            this.ServerUri = serverUri;
            this.SerialNumber = serialNumber;
            this.CompanyNumber = companyNumber;
        }

        public long SerialNumber { get; set; }

        public Uri ServerUri { get; private set; }

        public long CompanyNumber { get; private set; }

        public async Task<object> TryVerify()
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(ServerUri, new StringContent(CompanyNumber.ToString()));
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                return content;
            }
        }

        public async Task<bool> UpdateCompanyNumber()
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(ServerUri, new )
            } 
        }
    }
}
