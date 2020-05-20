using Prover.Application.Interfaces;
using Prover.UI.Desktop.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.UI.Desktop.Reports
{
	public class ReportViewModel : RoutableViewModelBase
	{
		public ReportViewModel(IScreenManager screen, ReactiveObject contentViewModel = null)
			: base(screen, "Report")
		{
			HostScreen = screen;
			ContentViewModel = contentViewModel;
		}

		[Reactive] public ReactiveObject ContentViewModel { get; set; }
	}
}
