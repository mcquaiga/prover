using System;
using Prover.Shared.Interfaces;

namespace Prover.Application.Interfaces
{
	public interface IApplicationServices
	{
		IVerificationService VerificationTestService { get; }
		ILoginService LoginService { get; }
		IServiceProvider Services { get; }
	}
}