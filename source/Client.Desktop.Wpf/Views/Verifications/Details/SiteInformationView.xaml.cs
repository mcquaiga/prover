using System;
using System.Drawing;
using System.Globalization;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Media;
using Client.Desktop.Wpf.Extensions;
using Devices.Core.Interfaces;
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

                //this.OneWayBind(ViewModel, vm => vm.Test.Verified, v => v.VerifiedStatusIcon.Foreground, value => value ? Brushes.ForestGreen : Brushes.IndianRed);

                DeviceNameTextBlock.Text = ViewModel.Device.DeviceType.Name;
                TestDateBlock.Text = $"{ViewModel.Test.TestDateTime:g}";

                JobIdTextBlock.Visibility = !string.IsNullOrEmpty(ViewModel.Test.JobId)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
                JobIdTextBlock.Text = $"Job #{ViewModel.Test.JobId ?? ""}";

                CompositionTypeTextBlock.Text = ViewModel.Device.CompositionShort();

                DriveTypeTextBlock.Text = Enum.GetName(typeof(VolumeInputType),
                    ViewModel.Test.VolumeTest.DriveType.InputType);

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
                    BasePressureTextBlock.Text = ViewModel.Pressure.Base.ToString(CultureInfo.InvariantCulture);
                    AtmPressureTextBlock.Text = ViewModel.Pressure.AtmosphericPressure.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    PressureInfoSection.Visibility = Visibility.Collapsed;
                }


                //Temperature
                if (ViewModel.Temperature != null)
                {
                    TemperatureInfoSection.Visibility = Visibility.Visible;
                    BaseTempTextBlock.Text = ViewModel.Temperature.Base.ToString(CultureInfo.InvariantCulture);
                    TempUnitsTextBlock.Text = ViewModel.Temperature.Units.ToString();
                }
                else
                {
                    TemperatureInfoSection.Visibility = Visibility.Collapsed;
                }


                //Pulse Outputs
                var channelA = ViewModel.PulseOutput.GetChannel(PulseOutputChannel.Channel_A);
                ChannelAUnitsTextBlock.Text = channelA.ChannelType.ToString();
                ChannelAScalingTextBlock.Text = channelA.Scaling.ToString(CultureInfo.InvariantCulture);

                var channelB = ViewModel.PulseOutput.GetChannel(PulseOutputChannel.Channel_B);
                ChannelBUnitsTextBlock.Text = channelB.ChannelType.ToString();
                ChannelBScalingTextBlock.Text = channelB.Scaling.ToString(CultureInfo.InvariantCulture);

                this.CleanUpDefaults().DisposeWith(d);
            });
        }
    }
}