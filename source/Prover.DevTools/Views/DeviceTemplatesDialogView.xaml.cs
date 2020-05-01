using System.Linq;
using System.Reactive.Disposables;
using ReactiveUI;

namespace Prover.Modules.DevTools.Views
{
	/// <summary>
	/// Interaction logic for DeviceTemplatesView.xaml
	/// </summary>
	public partial class DeviceTemplatesDialogView
	{
		public DeviceTemplatesDialogView()
		{
			InitializeComponent();

			this.WhenActivated(d =>
			{
				this.OneWayBind(ViewModel, vm => vm.DevicesCollection, v => v.DeviceTemplateItemsControl.ItemsSource,
				value => value.OrderBy(x => x.DeviceType.Name)).DisposeWith(d);

				this.Bind(ViewModel, vm => vm.SelectedDeviceInstance, v => v.DeviceTemplateItemsControl.SelectedItem).DisposeWith(d);

				this.BindCommand(ViewModel, vm => vm.CloseCommand, v => v.AcceptButton).DisposeWith(d);
				this.BindCommand(ViewModel, vm => vm.CancelCommand, v => v.CancelButton).DisposeWith(d);
			});
		}
	}
}
