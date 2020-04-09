using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Prover.Application.ViewModels;
using ReactiveUI;

namespace Prover.Application.Interfaces
{
    public enum VerificationTestStep
    {
        OnInitialize,
        OnSubmit,
        OnCorrections,
        OnVolume,
    }

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

    public interface IOnInitializeAction<in T> : IVerificationAction
        where T : IReactiveObject
    {
        Task OnInitialize(T item);
    }

    public interface IOnCompleteAction<in T> : IVerificationAction
        where T : IReactiveObject
    {
        Task OnComplete(T item);
    }
}