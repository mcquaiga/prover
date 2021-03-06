﻿using System.Reactive.Disposables;
using Prover.Application.ViewModels.Volume;
using Prover.UI.Desktop.Extensions;
using ReactiveUI;

namespace Prover.UI.Desktop.Views.QATests.Volume
{
    /// <summary>
    /// Interaction logic for UncorrectedVolumeView.xaml
    /// </summary>
    public partial class CorrectedVolumeView : ReactiveUserControl<CorrectedVolumeTestViewModel>
    {
        public CorrectedVolumeView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.PercentError, v => v.PercentErrorControl.DisplayValue).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Verified, v => v.PercentErrorControl.Passed).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.StartValues.CorrectedReading, v => v.StartReadingControl.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.EndReading, v => v.EndReadingControl.Value).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.ExpectedValue, v => v.ExpectedValueControl.Value).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ActualValue, v => v.ActualValueControl.Value).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.PulseOutputTest.ExpectedValue, v => v.PulsesExpectedControl.Value).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.PulseOutputTest.ActualValue, v => v.CorrectedPulseCountControl.Value).DisposeWith(d);
                //this.OneWayBind(ViewModel, vm => vm.PulseOutputTest.Verified, v => v.PulsesVerifiedControl.Value, 
                //    value => value ? "PASS" : "FAIL").DisposeWith(d);
                //this.OneWayBind(ViewModel, vm => vm.PulseOutputTest.Verified, v => v.PulsesVerifiedControl.Foreground, 
                //    value => value ? Brushes.Black : Brushes.DarkRed).DisposeWith(d);

                this.CleanUpDefaults().DisposeWith(d);
            });
        }
    }
}
