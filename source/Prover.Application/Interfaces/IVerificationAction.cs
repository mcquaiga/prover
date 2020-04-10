using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Prover.Application.ViewModels;
using ReactiveUI;

namespace Prover.Application.Interfaces
{
    public interface IEventsSubscriber
    {
        void SubscribeToEvents();
    }


    //public enum VerificationTestStep
    //{
    //    OnInitialize,
    //    OnSubmit,
    //    OnCorrections,
    //    OnVolume,
    //}

    public static class VerificationActionStages
    {
        //public static Dictionary<VerificationTestStep, Type> TestActionMappings =
        //    new Dictionary<VerificationTestStep, Type>()
        //    {
        //        {VerificationTestStep.OnInitialize, typeof(IOnInitializeAction)},
        //        {VerificationTestStep.OnSubmit, typeof(IOnCompleteAction)}
        //        //,
        //        //{VerificationTestStep.OnCorrectionTestStart,  typeof(IOnInitializeAction) },
        //        //{VerificationTestStep.OnCorrectionTestEnd,  typeof(IOnInitializeAction) },

        //    };
    }

    public interface IVerificationAction
    {
    }

    //public interface IOnVerificationStepAction : IVerificationAction
    //{
    //    VerificationTestStep VerificationStep { get; }
    //    Task OnStep(EvcVerificationViewModel verification);
    //}

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