using System;
using System.Collections.Generic;
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
using Application.ViewModels;
using Application.ViewModels.Corrections;
using ReactiveUI;

namespace Client.Wpf.Views.Verifications.Details
{
    /// <summary>
    /// Interaction logic for TemperatureTestView.xaml
    /// </summary>
    public partial class TemperatureTestView : ReactiveUserControl<TemperatureFactorViewModel>
    {
        public TemperatureTestView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.Bind(ViewModel, vm => vm.PercentError, v => v.PercentError.DisplayValue);
                this.Bind(ViewModel, vm => vm.Verified, v => v.PercentError.Passed);
                this.Bind(ViewModel, vm => vm.Gauge, v => v.Gauge.Value);
                this.Bind(ViewModel, vm => vm.Items.GasTemperature, v => v.EvcReading.Value);
                this.Bind(ViewModel, vm => vm.ActualValue, v => v.ActualFactor.Value);
                this.Bind(ViewModel, vm => vm.ExpectedValue, v => v.ExpectedFactor.Value);
            });
        }
    }
}
