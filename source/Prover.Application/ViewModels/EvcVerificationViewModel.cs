using Devices.Core.Interfaces;
using Prover.Application.ViewModels.Corrections;
using Prover.Application.ViewModels.Volume;
using Prover.Shared;
using Prover.Shared.Interfaces;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;

namespace Prover.Application.ViewModels
{
	public class EvcVerificationViewModel : VerificationViewModel
	{
		private EvcVerificationViewModel()
		{
		}

		private EvcVerificationViewModel(bool verified)
		{
		}

		[Reactive] public DeviceInstance Device { get; set; }

		[Reactive] public CompositionType CompositionType { get; set; }

		//[Reactive] public IVolumeInputType DriveType { get; set; }

		[Reactive] public DateTime TestDateTime { get; set; }

		[Reactive] public DateTime? ExportedDateTime { get; set; }

		[Reactive] public DateTime? SubmittedDateTime { get; set; }

		[Reactive] public DateTime? ArchivedDateTime { get; set; }

		[Reactive] public string JobId { get; set; }

		[Reactive] public string EmployeeId { get; set; }

		[Reactive] public string EmployeeName { get; set; }

		public ICollection<VerificationViewModel> VerificationTests { get; set; } = new List<VerificationViewModel>();

		//public VerificationResultViewModel Verification { get; set; }

		public SiteInformationViewModel DeviceInfo { get; set; }

		public VolumeViewModelBase VolumeTest => VerificationTests.OfType<VerificationTestPointViewModel>().FirstOrDefault(t => t.Volume != null)?.Volume;

		public void Initialize(ICollection<VerificationViewModel> verificationTests, ILoginService loginService = null)
		{
			DeviceInfo = new SiteInformationViewModel(Device, this, loginService);

			VerificationTests.Clear();
			VerificationTests.AddRange(verificationTests.ToArray());
			//Verification.VerificationTests = VerificationTests;
		}

		/// <param name="cleanup"></param>
		/// <inheritdoc />
		protected override void HandleActivation(CompositeDisposable cleanup)
		{
			base.HandleActivation(cleanup);

			RegisterVerificationsForVerified(VerificationTests);
		}

		protected override void Dispose(bool isDisposing)
		{
			VerificationTests.ForEach(t => t.DisposeWith(Cleanup));
			VerificationTests.Clear();
		}
	}

	//public class VerificationResultViewModel : VerificationViewModel
	//{
	//	//public ICollection<VerificationViewModel> VerificationTests { get; set; } = new List<VerificationViewModel>();

	//	//public VolumeViewModelBase VolumeTest => VerificationTests.OfType<VerificationTestPointViewModel>().FirstOrDefault(t => t.Volume != null)?.Volume;
	//}
}