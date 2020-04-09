using System;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Prover.Application.Verifications;
using Prover.Application.Verifications.CustomActions;
using Prover.Application.ViewModels;
using Prover.Application.ViewModels.Volume;

namespace Prover.Application.Interfaces
{
    public interface IActionsExecutioner
    {
        Task RunActionsOn<TOn>(EvcVerificationViewModel verificationTest) where TOn : IVerificationAction;

        //void RegisterAction<TOn>(TOn onAction);
        //void RegisterAction(VerificationTestStep onStep, Action<EvcVerificationViewModel> onAction);
        //void RegisterAction(Action<ICorrectionVerificationRunner> onAction, VerificationTestStep onStep = VerificationTestStep.OnCorrections);
        //void RegisterAction(Action<IVolumeTestManager> onAction, VerificationTestStep onStep = VerificationTestStep.OnVolume);
    }

    public interface IActivatable<T>
    {
        VerificationActivator<T> Activator { get; }
    }
}