using ReactiveUI;

namespace Prover.Modules.DevTools
{
	/// <summary>
	/// Interaction logic for DevMenuView.xaml
	/// </summary>
	public partial class DevMenuView
	{
		public DevMenuView()
		{
			InitializeComponent();

			this.WhenActivated(d =>
			{
				//this.OneWayBind(ViewModel, vm => vm.MenuItems, v => v.)
			});
		}
	}
}
