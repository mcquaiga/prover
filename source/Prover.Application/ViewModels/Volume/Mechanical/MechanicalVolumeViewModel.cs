using System.Collections.Generic;
using System.Linq;
using Devices.Core.Items.ItemGroups;
using Prover.Application.ViewModels.Corrections;
using Prover.Application.ViewModels.Factories.Volume;

namespace Prover.Application.ViewModels.Volume.Mechanical
{

	public class MechanicalVolumeViewModel : VolumeViewModelBase
	{
		public MechanicalVolumeViewModel(VolumeItems startVolumeItems, VolumeItems endVolumeItems)
				: base(startVolumeItems, endVolumeItems)
		{
		}

		public EnergyVolumeTestViewModel Energy => AllTests().OfType<EnergyVolumeTestViewModel>().FirstOrDefault();

		/// <inheritdoc />
		protected override ICollection<VerificationViewModel> GetSpecificTests()
		{
			return (ICollection<VerificationViewModel>)AllTests().OfType<EnergyVolumeTestViewModel>().ToList();
		}
	}
}