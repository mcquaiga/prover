using System;
using System.Reactive.Disposables;
using System.Windows;
using Client.Desktop.Wpf.Extensions;
using Prover.Application.ViewModels;
using Prover.Shared;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Verifications.Details
{
    /// <summary>
    ///     Interaction logic for SiteInformationView.xaml
    /// </summary>
    public partial class SiteInformationView : ReactiveUserControl<DeviceInfoViewModel>
    {
        public SiteInformationView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                DeviceNameTextBlock.Text = ViewModel.Device.DeviceType.Name;
                TestDateBlock.Text = ViewModel.TestDateTimePretty;
                //this.OneWayBind(ViewModel, vm => vm.VerificationTest.TestDateTime, v => v.TestDateBlock.Text, value => $"{value:g}").DisposeWith(d);

                CompositionTypeTextBlock.Text =
                    Enum.GetName(typeof(CompositionType), ViewModel.VerificationTest.CompositionType);

                DriveTypeTextBlock.Text = Enum.GetName(typeof(VolumeInputType),
                    ViewModel.VerificationTest.DriveType.InputType);

                CompanyNumberText.Text = ViewModel.CompanyNumber;
                SerialNumberText.Text = ViewModel.SiteInfo.SerialNumber;
                FirmwareText.Text = ViewModel.SiteInfo.FirmwareVersion;

                //Volume Info
                UncorUnitsTextBlock.Text = ViewModel.Volume.UncorrectedUnits;
                CorUnitsTextBlock.Text = ViewModel.Volume.CorrectedUnits;
                DriveRateTextBlock.Text = ViewModel.Volume.DriveRateDescription;


                //Pressure
                if (ViewModel.Pressure != null)
                {
                    PressureInfoSection.Visibility = Visibility.Visible;
                    PressureRangeTextBlock.Text = ViewModel.Pressure.Range.ToString();
                    PressureTransducerTextBlock.Text = ViewModel.Pressure.TransducerType.ToString();
                    BasePressureTextBlock.Text = ViewModel.Pressure.Base.ToString();
                    AtmPressureTextBlock.Text = ViewModel.Pressure.AtmosphericPressure.ToString();
                }
                else
                {
                    PressureInfoSection.Visibility = Visibility.Collapsed;
                }


                //Temperature
                if (ViewModel.Temperature != null)
                {
                    TemperatureInfoSection.Visibility = Visibility.Visible;
                    BaseTempTextBlock.Text = ViewModel.Temperature.Base.ToString();
                    TempUnitsTextBlock.Text = ViewModel.Temperature.Units.ToString();
                }
                else
                {
                    TemperatureInfoSection.Visibility = Visibility.Collapsed;
                }


                this.CleanUpDefaults().DisposeWith(d);
            });
        }
    }
}