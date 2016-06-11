using System;
using System.Threading.Tasks;
using Caliburn.Micro.ReactiveUI;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Models.Instruments;

namespace Prover.Core.ExternalIntegrations
{
    public interface IVerifier
    {
        Task<object> Verify(Instrument instrument);
        Task<bool> Update(EvcCommunicationClient commClient, Instrument instrument, long newCompanyNumber);
        VerificationNotValidEvent VerificationNotValid { get; }
    }

    public class VerificationNotValidEvent
    {
        public VerificationNotValidEvent(ItemValue item, ReactiveScreen viewModel)
        {
            Item = item;
            this.ViewModel = viewModel;
        }

        public ReactiveScreen ViewModel { get; set; }

        public ItemValue Item { get; set; }
    }
}