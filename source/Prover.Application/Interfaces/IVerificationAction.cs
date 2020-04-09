using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Prover.Application.ViewModels;

namespace Prover.Application.Interfaces
{
    public enum VerificationTestStep
    {
        OnInitialize,
        OnSubmit,
        OnCorrectionTestStart,
        OnCorrectionTestEnd,
        OnVolumeStart,
        OnVolumeEnd
    }

    public static class VerificationActionStages
    {
        public static Type OnInitialize = ;
        public static Type OnSubmit = typeof(IOnSubmitAction);
        public static Type OnCorrectionTestStart = typeof(IOnInitializeAction);
        public static Type OnVolumeTestStart = typeof(IOnInitializeAction);
        public static Type OnVolumeTestEnd = typeof(IOnInitializeAction);

        public static Dictionary<VerificationTestStep, Type> TestActionMappings = new Dictionary<VerificationTestStep, Type>()
        {
            {VerificationTestStep.OnInitialize,  typeof(IOnInitializeAction) },
            {VerificationTestStep.OnSubmit,  typeof(IOnSubmitAction) }
            //,
            //{VerificationTestStep.OnCorrectionTestStart,  typeof(IOnInitializeAction) },
            //{VerificationTestStep.OnCorrectionTestEnd,  typeof(IOnInitializeAction) },

        }
    }

    public interface IVerificationAction
    {
    }

    public interface IOnVerificationStepAction : IVerificationAction
    {
        VerificationTestStep VerificationStep { get; }
        Task OnStep(EvcVerificationViewModel verification);
    }

    public interface IOnInitializeAction : IVerificationAction
    {
        Task OnInitialize(EvcVerificationViewModel verification);
    }

    public interface IOnSubmitAction : IVerificationAction
    {
        Task OnSubmit(EvcVerificationViewModel verification);
        //new Task Execute(EvcVerificationViewModel verification);
    }
}