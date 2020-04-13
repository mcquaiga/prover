using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Prover.Application.Services.LiveReadCorrections;
using Prover.Application.Verifications.Events;
using Prover.Application.ViewModels;

namespace Prover.Application.Verifications
{
    public class VerificationEvents
    {
        public static VerificationEvent<EvcVerificationViewModel, EvcVerificationViewModel> 
                OnInitialize { get; } = new VerificationEvent<EvcVerificationViewModel, EvcVerificationViewModel>();

        public static VerificationEvent<EvcVerificationViewModel, EvcVerificationViewModel> 
                OnSubmit { get; } = new VerificationEvent<EvcVerificationViewModel, EvcVerificationViewModel>(); 
        
        public static VerificationEvent<EvcVerificationViewModel, EvcVerificationViewModel> 
                OnVerified { get; } = new VerificationEvent<EvcVerificationViewModel, EvcVerificationViewModel>();


        public static class CorrectionTests
        {
            public static VerificationEvent<VerificationTestPointViewModel, VerificationTestPointViewModel> 
                    OnStart { get; } = new VerificationEvent<VerificationTestPointViewModel, VerificationTestPointViewModel>();

            public static VerificationEvent<VerificationTestPointViewModel, VerificationTestPointViewModel> 
                    OnComplete { get; } = new VerificationEvent<VerificationTestPointViewModel, VerificationTestPointViewModel>();

            public static VerificationEvent<LiveReadCoordinator, LiveReadCoordinator> 
                    OnLiveReadStart { get; } = new VerificationEvent<LiveReadCoordinator, LiveReadCoordinator>(); 
            
            public static VerificationEvent<LiveReadCoordinator, LiveReadCoordinator> 
                    OnLiveReadComplete { get; } = new VerificationEvent<LiveReadCoordinator, LiveReadCoordinator>();
        }

        public static void DefaultSubscribers(ILogger<VerificationEvents> logger = null)
        {
            logger = logger ?? NullLogger<VerificationEvents>.Instance;
            
            OnInitialize.Subscribe(e => SetResponse(e, e.Input, nameof(OnInitialize)));
            OnSubmit.Subscribe(e => SetResponse(e, e.Input, nameof(OnSubmit)));
            
            CorrectionTests.OnStart.Subscribe(e => SetResponse(e, e.Input, nameof(CorrectionTests.OnStart)));
            CorrectionTests.OnComplete.Subscribe(e => SetResponse(e, e.Input, nameof(CorrectionTests.OnComplete)));

            void SetResponse<TIn, TOut>(EventContext<TIn, TOut> context, TOut output, string name)
            {
                logger.LogDebug($"{name} Default event {name} was raised.");
                context.SetOutput(output);
            }
        }

    }
}