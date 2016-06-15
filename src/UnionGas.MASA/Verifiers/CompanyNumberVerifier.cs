using System;
using System.Net.Http;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Instruments;
using Prover.GUI.Common;
using UnionGas.MASA.Dialogs.CompanyNumberDialog;

namespace UnionGas.MASA.Verifiers
{
    public class CompanyNumberVerifier : IVerifier
    {
        private ItemValue _companyNumber;
        private const string ServerUrl = "http://uniongas.masa/";
        private const string VerifyEndPoint = "Verify";

        public CompanyNumberVerifier(IUnityContainer container, EvcCommunicationClient commClient, Instrument instrument)
        {
            this.Container = container;
            this.CommClient = commClient;
            this.Instrument = instrument;
        }

        public IUnityContainer Container { get; set; }

        public Instrument Instrument { get; set; }

        public EvcCommunicationClient CommClient { get; set; }

        public async Task<object> Verify()
        {
            _companyNumber = Instrument.Items.GetItem(ItemCodes.SiteInfo.CompanyNumber);

            var viewModel = new CompanyNumberDialogViewModel(this);
            ScreenManager.ShowDialog(Container, viewModel);

            return await Update(viewModel.CompanyNumber);

            var serverUri = new Uri(ServerUrl);
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(serverUri, new StringContent(_companyNumber.ToString()));
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                return content;
            }
        }

        public async Task<bool> Update(string newCompanyNumber)
        {
            await CommClient.Connect();
            var response = await CommClient.SetItemValue(ItemCodes.SiteInfo.CompanyNumber, long.Parse(newCompanyNumber));
            await CommClient.Disconnect();

            if (response)
                Instrument.Items.GetItem(ItemCodes.SiteInfo.CompanyNumber).RawValue = newCompanyNumber.ToString();

            return response;
        }
    }
}
