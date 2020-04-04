using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Prover.Application.ViewModels;

namespace Prover.Application.Interfaces
{
    public enum VerificationTestStep
    {
        OnInitialize,
        OnSubmit,
        OnVolumeStart,
        OnVolumeEnd
    }

    public interface IVerificationAction
    {
        //Task Execute(EvcVerificationViewModel verification);
    }

    public interface IInitializeAction : IVerificationAction
    {
        Task OnInitialize(EvcVerificationViewModel verification);
        //new Task Execute(EvcVerificationViewModel verification);
    }

    public interface ISubmitAction : IVerificationAction
    {
        Task OnSubmit(EvcVerificationViewModel verification);
        //new Task Execute(EvcVerificationViewModel verification);
    }
}