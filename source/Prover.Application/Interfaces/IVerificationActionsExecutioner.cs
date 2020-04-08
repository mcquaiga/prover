using System.Threading.Tasks;
using Devices.Core.Interfaces;
using Prover.Application.ViewModels;

namespace Prover.Application.Interfaces
{
    public interface IActionsExecutioner
    {
        Task RunActionsOn<TOn>(EvcVerificationViewModel verificationTest) where TOn : IVerificationAction;
    }
}