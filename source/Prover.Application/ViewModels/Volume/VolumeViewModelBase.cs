using Devices.Core.Items.ItemGroups;
using Prover.Application.ViewModels.Corrections;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;

namespace Prover.Application.ViewModels.Volume
{
	public abstract class VolumeViewModelBase : VerificationViewModel, IDeviceStartAndEndValues<VolumeItems>
	{
		private ICollection<VerificationViewModel> _allTests = new List<VerificationViewModel>();

		protected VolumeViewModelBase(VolumeItems startValues, VolumeItems endValues)
		{
			Id = Guid.Empty;

			StartValues = startValues;
			EndValues = endValues;
		}

		[Reactive] public VolumeItems StartValues { get; set; }
		[Reactive] public VolumeItems EndValues { get; set; }

		public CorrectedVolumeTestViewModel Corrected => AllTests().OfType<CorrectedVolumeTestViewModel>().FirstOrDefault();
		public UncorrectedVolumeTestViewModel Uncorrected => AllTests().OfType<UncorrectedVolumeTestViewModel>().FirstOrDefault();

		//public ReactiveCommand<Unit, Unit> StartTest { get; protected set; }
		//public ReactiveCommand<Unit, Unit> FinishTest { get; protected set; }

		//public virtual IVolumeInputType DriveType { get; set; }

		public virtual ICollection<VerificationViewModel> AllTests() => _allTests;

		protected override void Dispose(bool isDisposing)
		{
			AllTests().ForEach(t => t.DisposeWith(Cleanup));
		}

		public void AddVerificationTest(VerificationViewModel verification)
		{
			_allTests.Add(verification);
		}

		/// <param name="cleanup"></param>
		/// <inheritdoc />
		protected override void HandleActivation(CompositeDisposable cleanup)
		{
			base.HandleActivation(cleanup);

			RegisterVerificationsForVerified(_allTests);
		}

		protected abstract ICollection<VerificationViewModel> GetSpecificTests();
	}
}