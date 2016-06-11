using System;
using System.Net.Http;
using System.Threading.Tasks;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Instruments;
using UnionGas.MASA.Dialogs.CompanyNumberDialog;

namespace UnionGas.MASA.Verifiers
{
    public class CompanyNumberVerifier : IVerifier
    {
        private ItemValue _companyNumber;
        private const string ServerUrl = "http://uniongas.masa/";
        private const string VerifyEndPoint = "Verify";

        public async Task<object> Verify(Instrument instrument)
        {
            var serverUri = new Uri(ServerUrl);

            _companyNumber = instrument.Items.GetItem(ItemCodes.SiteInfo.CompanyNumber);

            return null;
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(serverUri, new StringContent(_companyNumber.ToString()));
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                return content;
            }
        }

        public async Task<bool> Update(EvcCommunicationClient commClient, Instrument instrument, long newCompanyNumber)
        {
            await commClient.Connect();
            var response = await commClient.SetItemValue(ItemCodes.SiteInfo.CompanyNumber, newCompanyNumber);
            await commClient.Disconnect();

            if (response)
                instrument.Items.GetItem(ItemCodes.SiteInfo.CompanyNumber).RawValue = newCompanyNumber.ToString();

            return response;
        }

        public VerificationNotValidEvent VerificationNotValid => new VerificationNotValidEvent(_companyNumber, new CompanyNumberDialogViewModel());
    }
}
