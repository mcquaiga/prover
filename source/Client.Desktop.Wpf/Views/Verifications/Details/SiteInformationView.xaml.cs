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

                CompanyNumberText.Content = ViewModel.CompanyNumber;
                SerialNumberText.Content = ViewModel.SiteInfo.SerialNumber;
                FirmwareText.Content = ViewModel.SiteInfo.FirmwareVersion;

                //Volume Info
                UncorUnitsTextBlock.Content = ViewModel.Volume.UncorrectedUnits;
                CorUnitsTextBlock.Content = ViewModel.Volume.CorrectedUnits;
                DriveRateTextBlock.Content = ViewModel.Volume.DriveRateDescription;


                //Pressure
                if (ViewModel.Pressure != null)
                {
                    PressureInfoSection.Visibility = Visibility.Visible;
                    PressureRangeTextBlock.Content = ViewModel.Pressure.Range.ToString();
                    PressureTransducerTextBlock.Content = ViewModel.Pressure.TransducerType.ToString();
                    BasePressureTextBlock.Content = ViewModel.Pressure.Base.ToString(CultureInfo.InvariantCulture);
                    AtmPressureTextBlock.Content = ViewModel.Pressure.AtmosphericPressure.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    PressureInfoSection.Visibility = Visibility.Collapsed;
                }


                //Temperature
                if (ViewModel.Temperature != null)
                {
                    TemperatureInfoSection.Visibility = Visibility.Visible;
                    BaseTempTextBlock.Content = ViewModel.Temperature.Base.ToString(CultureInfo.InvariantCulture);
                    TempUnitsTextBlock.Content = ViewModel.Temperature.Units.ToString();
                }
                else
                {
                    TemperatureInfoSection.Visibility = Visibility.Collapsed;
                }

                //Pulse Outputs
                SetPulseOutputChannel(PulseOutputChannel.Channel_A, ChannelAContentControl, ChannelAUnitsTextBlock);
                SetPulseOutputChannel(PulseOutputChannel.Channel_B, ChannelBContentControl, ChannelBUnitsTextBlock);

                this.CleanUpDefaults().DisposeWith(d);
            });
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