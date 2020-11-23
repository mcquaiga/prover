using Prover.CommProtocol.Common;
using Prover.Core.Models.Instruments;
using Prover.Core.VerificationTests.TestActions;
using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace UnionGas.MASA.Validators {
	public class BarCodeValidator : IEvcDeviceValidationAction {
		public VerificationStep VerificationStep => throw new NotImplementedException();

		public Task Execute(EvcCommunicationClient commClient, Instrument instrument, CancellationToken ct = new CancellationToken(), Subject<string> statusUpdates = null) {
			throw new NotImplementedException();
		}
	}
}
