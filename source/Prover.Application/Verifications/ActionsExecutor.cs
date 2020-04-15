//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Logging.Abstractions;
//using Prover.Application.Interfaces;
//using Prover.Application.ViewModels;

//namespace Prover.Application.Verifications
//{
//    public class VerificationActionsExecutor : IActionsExecutioner
//    {
//        private readonly IDeviceSessionManager _deviceSessionManager;


//        //private readonly IEnumerable<IInitializeAction> _onInitializeActions;
//        //private readonly IEnumerable<ISubmitAction> _onSubmitActions;
//        private readonly ILogger<VerificationActionsExecutor> _logger;
//        private readonly IEnumerable<IVerificationAction> _verificationActions;

//        public VerificationActionsExecutor(
//            IDeviceSessionManager deviceSessionManager, 
//            IEnumerable<IVerificationAction> verificationActions,
//            ILogger<VerificationActionsExecutor> logger = null)
//        {
//            _logger = logger ?? NullLogger<VerificationActionsExecutor>.Instance;

//            _deviceSessionManager = deviceSessionManager;
//            _verificationActions = verificationActions;
//        }

//        public async Task RunActionsOn<TOn>(EvcVerificationViewModel verificationTest)
//            where TOn : IVerificationAction
//        {
//            var onType = typeof(TOn);

//            await _deviceSessionManager.Connect();

//            foreach (var verificationAction in _verificationActions.OfType<TOn>())
//            {
//                if (typeof(TOn) == typeof(IOnInitializeAction) && verificationAction is IOnInitializeAction init) 
//                    await init.OnInitialize(verificationTest);

//                if (typeof(TOn) == typeof(IOnSubmitAction) && verificationAction is IOnSubmitAction submit) 
//                    await submit.OnSubmit(verificationTest);
                
//            }
//        }

//        public void RegisterAction<TOn>(TOn onAction)
//        {
//            throw new NotImplementedException();
//        }

//        public void RegisterAction(Action<EvcVerificationViewModel> onAction)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}