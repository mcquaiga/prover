using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Domain.Model.Instrument;
using Prover.Domain.Model.Verifications;
using Prover.InstrumentProtocol.Core.Factories;
using Prover.InstrumentProtocol.Core.Models.Instrument;
using Prover.Shared.Data;

namespace Prover.Services.Verification
{
    public class VerificationCoordinatorService
    {
        private readonly IVerificationRunProviderService _verificationRunProviderService;
        private readonly IVerificationRunFactory _verificationRunFactory;

        public VerificationCoordinatorService(IVerificationRunProviderService verificationRunProviderService, 
            IVerificationRunFactory verificationRunFactory)
        {
            _verificationRunProviderService = verificationRunProviderService;
            _verificationRunFactory = verificationRunFactory;
        }

        public async Task InitializeVerificationRun(IInstrumentFactory instrumentFactory)
        {
            var run = await _verificationRunFactory.Create(instrumentFactory);
            
        }
    }
}
