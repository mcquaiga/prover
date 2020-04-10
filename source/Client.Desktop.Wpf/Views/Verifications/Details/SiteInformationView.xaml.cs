using System;
using System.Globalization;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Client.Desktop.Wpf.Extensions;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using MaterialDesignThemes.Wpf;
using Prover.Application.ViewModels;
using Prover.Shared;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Verifications.Details
{
    /// <summary>
    ///     Interaction logic for SiteInformationView.xaml
    /// </summary>
    public partial class SiteInformationView
    {
        public SiteInformationView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.Test.Verified, v => v.VerifiedStatusIcon.Kind,
                                value => value ? PackIconKind.Check : PackIconKind.AlertCircleOutline).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Test.Verified, v => v.VerifiedStatusIcon.Foreground,
                                value => value ? Brushes.ForestGreen : Brushes.IndianRed).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.EmployeeName, v => v.EmployeeIdTextControl.Content).DisposeWith(d);

                SetWithViewModel(ViewModel);

                //Pulse Outputs
                SetPulseOutputChannel(PulseOutputChannel.Channel_A, ChannelAContentControl, ChannelAUnitsTextBlock);
                SetPulseOutputChannel(PulseOutputChannel.Channel_B, ChannelBContentControl, ChannelBUnitsTextBlock);

                this.CleanUpDefaults().DisposeWith(d);
            });
        }

        private void SetPulseOutputChannel(PulseOutputChannel channel, ContentControl channelControl,
            ContentControl unitsControl)
        {
            var channelItems = ViewModel.PulseOutput.GetChannel(channel);
            channelControl.DataContext = channelItems;

            if (channelItems is IVolumeUnits units)
            {
                unitsControl.Visibility = Visibility.Visible;
                unitsControl.Content = units.Units.Description;
                return;
            }

            unitsControl.Visibility = Visibility.Collapsed;
        }

        private void SetWithViewModel(SiteInformationViewModel viewModel)
        {
            DeviceNameTextBlock.Text = viewModel.Device.DeviceType.Name;
            TestDateBlock.Text = $"{viewModel.TestDateTime:g}";

            JobIdTextControl.Visibility = !string.IsNullOrEmpty(viewModel.Test.JobId)
                                              ? Visibility.Visible
                                              : Visibility.Collapsed;
            JobIdTextControl.Content = viewModel.Test.JobId;

            CompositionTypeTextBlock.Text = viewModel.Device.CompositionShort();

            DriveTypeTextBlock.Text = Enum.GetName(typeof(VolumeInputType),
                                                   viewModel.Test.VolumeTest.DriveType.InputType);

            CompanyNumberText.Content = viewModel.CompanyNumber;
            SerialNumberText.Content = viewModel.SiteInfo.SerialNumber;
            FirmwareText.Content = viewModel.SiteInfo.FirmwareVersion;

            //Volume Info
            UncorUnitsTextBlock.Content = viewModel.Volume.UncorrectedUnits;
            CorUnitsTextBlock.Content = viewModel.Volume.CorrectedUnits;
            DriveRateTextBlock.Content = viewModel.Volume.DriveRateDescription;


            //Pressure
            PressureInfoSection.Visibility = viewModel.Pressure != null ? Visibility.Visible : Visibility.Collapsed;
            if (viewModel.Pressure != null)
            {
                PressureUnitsTextBlock.Content = viewModel.Pressure.UnitType.ToString();
                PressureRangeTextBlock.Content = viewModel.Pressure.Range.ToString();
                PressureTransducerTextBlock.Content = viewModel.Pressure.TransducerType.ToString();
                BasePressureTextBlock.Content = viewModel.Pressure.Base.ToString(CultureInfo.InvariantCulture);
                AtmPressureTextBlock.Content =
                    viewModel.Pressure.AtmosphericPressure.ToString(CultureInfo.InvariantCulture);
            }

            //Temperature
            TemperatureInfoSection.Visibility =
                viewModel.Temperature != null ? Visibility.Visible : Visibility.Collapsed;
            BaseTempTextBlock.Content = viewModel.Temperature?.Base.ToString(CultureInfo.InvariantCulture);
            TempUnitsTextBlock.Content = viewModel.Temperature?.Units.ToString();
        }
    }
}

//this.OneWayBind(ViewModel, vm => vm.Test.EmployeeId, v => v.EmployeeTextBlock.Visibility,
//    value =>
//        !string.IsNullOrEmpty(value)
//            ? Visibility.Visible
//            : Visibility.Collapsed)
//    .DisposeWith(d);