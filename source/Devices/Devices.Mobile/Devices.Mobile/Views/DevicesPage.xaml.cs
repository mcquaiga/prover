using Devices.Mobile.Services;
using Devices.Mobile.ViewModels;
using ReactiveUI;
using ReactiveUI.XamForms;
using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Devices.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DevicesPage : ReactiveContentPage<DevicesViewModel>
    {
        public DevicesPage()
        {
            ViewModel = new DevicesViewModel();

            InitializeComponent();

            this.WhenActivated(disp =>
            {
                BindingContext = ViewModel;

                //this.OneWayBind(ViewModel, x => x.PortNames, x => x.PortNamesListView);

                //disp(this.BindCommand(this.ViewModel, x => x.GetPortsCommand, x => x.RefreshPortsButton));

                this.WhenAnyValue(x => x.ViewModel)
                    .Select(x => x.GetDevicesCommand.Execute())
                    .Subscribe()
                    .DisposeWith(disp);
            });
        }

        private async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as DeviceGetDto;
            if (item == null)
                return;

            await Navigation.PushAsync(new PortsPage(new PortsViewModel(item)));

            // Manually deselect item.
            DeviceNamesListView.SelectedItem = null;
        }
    }
}