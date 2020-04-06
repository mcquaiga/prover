using System;
using System.Drawing;
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
using Brushes = System.Windows.Media.Brushes;

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
                this.OneWayBind(ViewModel, vm => vm.Test.Verified, v => v.VerifiedStatusIcon.Kind, value => value ? PackIconKind.Check : PackIconKind.AlertCircleOutline).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Test.Verified, v => v.VerifiedStatusIcon.Foreground, value => value ? Brushes.ForestGreen : Brushes.IndianRed).DisposeWith(d);
                
                this.OneWayBind(ViewModel, vm => vm.Test.EmployeeId, v => v.EmployeeTextBlock.Text, value => $"Employee #{value}").DisposeWith(d);

                //this.OneWayBind(ViewModel, vm => vm.Test.Verified, v => v.VerifiedStatusIcon.Foreground, value => value ? Brushes.ForestGreen : Brushes.IndianRed);

                SetWithViewModel(ViewModel);

                //Pulse Outputs
                SetPulseOutputChannel(PulseOutputChannel.Channel_A, ChannelAContentControl, ChannelAUnitsTextBlock);
                SetPulseOutputChannel(PulseOutputChannel.Channel_B, ChannelBContentControl, ChannelBUnitsTextBlock);

                this.CleanUpDefaults().DisposeWith(d);
            });
        }

        private void SetWithViewModel(SiteInformationViewModel viewModel)
        {
            DeviceNameTextBlock.Text = viewModel.Device.DeviceType.Name;
            TestDateBlock.Text = $"{viewModel.TestDateTime:g}";

            JobIdTextBlock.Visibility = !string.IsNullOrEmpty(viewModel.Test.JobId)
                ? Visibility.Visible
                : Visibility.Collapsed;
            JobIdTextBlock.Text = $"Job #{viewModel.Test.JobId ?? ""}";

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
            if (viewModel.Pressure != null)
            {
                PressureInfoSection.Visibility = Visibility.Visible;
                PressureRangeTextBlock.Content = viewModel.Pressure.Range.ToString();
                PressureTransducerTextBlock.Content = viewModel.Pressure.TransducerType.ToString();
                BasePressureTextBlock.Content = viewModel.Pressure.Base.ToString(CultureInfo.InvariantCulture);
                AtmPressureTextBlock.Content = viewModel.Pressure.AtmosphericPressure.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                PressureInfoSection.Visibility = Visibility.Collapsed;
            }


            //Temperature
            if (viewModel.Temperature != null)
            {
                TemperatureInfoSection.Visibility = Visibility.Visible;
                BaseTempTextBlock.Content = viewModel.Temperature.Base.ToString(CultureInfo.InvariantCulture);
                TempUnitsTextBlock.Content = viewModel.Temperature.Units.ToString();
            }
            else
            {
                TemperatureInfoSection.Visibility = Visibility.Collapsed;
            }
        }

        private void SetPulseOutputChannel(PulseOutputChannel channel, ContentControl channelControl, ContentControl unitsControl)
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

    }
}