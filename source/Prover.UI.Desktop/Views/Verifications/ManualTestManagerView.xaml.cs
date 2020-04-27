using ReactiveUI;
using System.Reactive.Disposables;

namespace Prover.UI.Desktop.Views.Verifications
{
	/// <summary>
	/// Interaction logic for ManualTestManagerView.xaml
	/// </summary>
	public partial class ManualTestManagerView
	{
		public ManualTestManagerView()
		{
			InitializeComponent();

			this.WhenActivated(d =>
			{
				this.OneWayBind(ViewModel, vm => vm.TestViewModel, v => v.TestViewContent.ViewModel).DisposeWith(d);


			});
		}
	}
}
