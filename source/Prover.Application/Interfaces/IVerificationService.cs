using System;
using Devices.Core.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using DynamicData;
using System.Linq.Expressions;
using Prover.Application.Services;

namespace Prover.Application.Interfaces
{
	public interface IVerificationService //: ICacheAggregateRoot<EvcVerificationTest>
	{
		Task<EvcVerificationViewModel> Save(EvcVerificationViewModel viewModel);
		Task<EvcVerificationTest> Save(EvcVerificationTest evcVerificationTest);
		Task Save(IEnumerable<EvcVerificationTest> evcVerificationTest);

		VerificationQueries Query { get; }

		Task<EvcVerificationTest> Archive(EvcVerificationTest model);

		Task<IQaTestRunManager> StartVerification(DeviceType deviceType, VerificationTestOptions options = null);
		Task<IQaTestRunManager> StartVerification(DeviceInstance device, VerificationTestOptions options = null, bool publishEvent = false);

		Task<bool> CompleteVerification(EvcVerificationViewModel viewModel);
	}
}