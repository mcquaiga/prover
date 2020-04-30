using Prover.Application.ViewModels.Volume.Rotary;
using ReactiveUI;

namespace Prover.UI.Desktop.Views.Verifications.Volume
{
	public class RotaryUncorrectedVolumeView : UncorrectedVolumeView, IViewFor<RotaryUncorrectedVolumeTestViewModel>
	{
		//public RotaryUncorrectedVolumeView()
		//{
		//    this.WhenActivated(d =>
		//    {
		//        //ViewModel = base.ViewModel as RotaryUncorrectedVolumeTestViewModel;
		//    });
		//}

		RotaryUncorrectedVolumeTestViewModel IViewFor<RotaryUncorrectedVolumeTestViewModel>.ViewModel { get; set; }

	}
}