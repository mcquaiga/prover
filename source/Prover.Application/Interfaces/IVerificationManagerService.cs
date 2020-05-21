using Devices.Core.Interfaces;
using System.Threading.Tasks;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.ViewModels;

namespace Prover.Application.Interfaces
{
	public interface IVerificationManagerService
	{
		IQaTestRunManager CreateManager(EvcVerificationTest verificationTest);
		//Task<bool> CompleteVerification(EvcVerificationViewModel viewModel);
	}
}