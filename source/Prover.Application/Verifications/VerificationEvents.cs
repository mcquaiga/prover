using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
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


        public static class CorrectionTest
        {
            public static VerificationEvent<VerificationTestPointViewModel, VerificationTestPointViewModel> 
                    OnStart { get; } = new VerificationEvent<VerificationTestPointViewModel, VerificationTestPointViewModel>();

            public static VerificationEvent<VerificationTestPointViewModel, VerificationTestPointViewModel> 
                    OnFinish { get; } = new VerificationEvent<VerificationTestPointViewModel, VerificationTestPointViewModel>();
        }

        public static void DefaultSubscribers(ILogger<VerificationEvents> logger = null)
        {
            logger = logger ?? NullLogger<VerificationEvents>.Instance;
            
            OnInitialize.Subscribe(e => SetResponse(e, e.Input, nameof(OnInitialize)));
            OnSubmit.Subscribe(e => SetResponse(e, e.Input, nameof(OnSubmit)));
            
            CorrectionTest.OnStart.Subscribe(e => SetResponse(e, e.Input, nameof(CorrectionTest.OnStart)));
            CorrectionTest.OnFinish.Subscribe(e => SetResponse(e, e.Input, nameof(CorrectionTest.OnFinish)));

            void SetResponse<TIn, TOut>(EventContext<TIn, TOut> context, TOut output, string name)
            {
                logger.LogDebug($"{name} Default event {name} was raised.");
                context.SetOutput(output);
            }
        }

    }
}