using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Verifications.Details.DriveTypes
{
    /// <summary>
    /// Interaction logic for RotaryMeterView.xaml
    /// </summary>
    public partial class RotaryMeterView
    {
        public RotaryMeterView()
        {
            InitializeComponent();
            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.PercentError, v => v.RotaryPercentErrorControl.DisplayValue).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Verified, v => v.RotaryPercentErrorControl.Passed).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.ActualValue, v => v.RotaryEvcMeterDisplacementControl.Value).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.ExpectedValue, v => v.RotaryExpectedMeterDisplacementControl.Value).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Items.MeterType, v => v.RotaryMeterTypeText.Text).DisposeWith(d);
            });
        }
    }
}
