using Devices.Mobile.ViewModels;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.XamForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Devices.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewConnectionView : ReactiveContentPage<NewConnectionViewModel>
    {
        public NewConnectionView(NewConnectionViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();

            this.WhenActivated(disposable =>
            {
                BindingContext = ViewModel;

                //Connect Button
                this.BindCommand(this.ViewModel, x => x.CreateConnectionCommand, x => x.ConnectButton)
                    .DisposeWith(disposable);

                this.BindCommand(this.ViewModel, x => x.DisconnectCommand, x => x.DisconnectButton)
                    .DisposeWith(disposable);

                this.BindCommand(this.ViewModel, x => x.PressureItemsCommand, x => x.FetchPressureItemsButton)
                   .DisposeWith(disposable);
            });
        }
    }
}