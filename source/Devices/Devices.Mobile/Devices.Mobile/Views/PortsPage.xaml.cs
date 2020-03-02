using Devices.Mobile.ViewModels;
using ReactiveUI;
using ReactiveUI.XamForms;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.TizenSpecific;
using Xamarin.Forms.PlatformConfiguration.WindowsSpecific;
using Xamarin.Forms.Xaml;

namespace Devices.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PortsPage : ReactiveContentPage<PortsViewModel>
    {
        public PortsPage() : this(new PortsViewModel())
        {
        }

        public PortsPage(PortsViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();

            this.WhenActivated(disp =>
            {
                BindingContext = ViewModel;

                this.OneWayBind(ViewModel, x => x.PortNames, x => x.PortNamesListView.ItemsSource)
                    .DisposeWith(disp);

                //disp(this.BindCommand(this.ViewModel, x => x.GetPortsCommand, x => x.RefreshPortsButton));

                this.WhenAnyValue(x => x.ViewModel)
                    .Select(x => x.GetPortsCommand.Execute())
                    .Subscribe()
                    .DisposeWith(disp);
            });
        }

        private async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as string;
            if (item == null)
                return;

            ViewModel.SelectedPort = item;

            await Navigation.PushAsync(
                new NewConnectionView(
                    new NewConnectionViewModel(ViewModel.Device, ViewModel.SelectedPort)
                ));
        }
    }
}