using System;
using System.Net.Http;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;

namespace UnionGas.MASA.Verifiers
{
    public static class CompanyNumberVerifier
    {
        private const string VerifyEndPoint = "Verify";
        private const string UpdateEndPoint = "Update";

        public static async Task<object> VerifyCompanyNumber(Uri serverUri, long companyNumber)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(serverUri, new StringContent(companyNumber.ToString()));
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                return content;
            }
        }

        public static async Task<bool> UpdateCompanyNumber(EvcCommunicationClient commClient, long newCompanyNumber)
        {
            await commClient.Connect();
            return await commClient.SetItemValue(ItemCodes.SiteInfo.CompanyNumber, newCompanyNumber);
        }
    }
}
