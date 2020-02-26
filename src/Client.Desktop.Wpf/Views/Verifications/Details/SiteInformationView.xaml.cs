using System;
using System.Reactive.Disposables;
using Prover.Application.ViewModels;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Verifications.Details
{
    /// <summary>
    /// Interaction logic for SiteInformationView.xaml
    /// </summary>
    public partial class SiteInformationView : ReactiveUserControl<DeviceInfoViewModel>
    {
        public SiteInformationView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
                {
                    //this.OneWayBind(ViewModel, vm => vm.Test.TestDateTime, v => v.TestDateBlock.Text, value => $"{value:g}").DisposeWith(d);
                    
                    this.OneWayBind(ViewModel, vm => vm.SiteInfo.CompositionType, v => v.CompositionType.Text, val => Enum.GetName(val.GetType(), val)).DisposeWith(d);
                    //this.OneWayBind(ViewModel, vm => vm.Test.DriveType.InputType, v => v.DriveType.Text, val => Enum.GetName(val.GetType(), val)).DisposeWith(d);

                    this.OneWayBind(ViewModel, vm => vm.SiteInfo.SiteId2, v => v.SiteId2Text.Text).DisposeWith(d);
                    this.OneWayBind(ViewModel, vm => vm.SiteInfo.SerialNumber, v => v.SerialNumberText.Text).DisposeWith(d);
                    this.OneWayBind(ViewModel, vm => vm.SiteInfo.FirmwareVersion, v => v.FirmwareText.Text).DisposeWith(d);
                });
        }
    }
}
