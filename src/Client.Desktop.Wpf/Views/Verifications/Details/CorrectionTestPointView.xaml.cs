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
using Client.Wpf.ViewModels.Verifications;
using ReactiveUI;

namespace Client.Wpf.Views.Verifications.Details
{
    /// <summary>
    /// Interaction logic for CorrectionTestPointView.xaml
    /// </summary>
    public partial class CorrectionTestPointView : ReactiveUserControl<VerificationTestPointViewModel>
    {
        public CorrectionTestPointView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
                {
                    this.OneWayBind(ViewModel, vm => vm.TestNumber, v => v.TestLevelBlock.Text, value => $"Level {value+1}");
                    this.OneWayBind(ViewModel, vm => vm.Pressure, v => v.PressureContent.ViewModel);
                    this.OneWayBind(ViewModel, vm => vm.Temperature, v => v.TemperatureContent.ViewModel);
                    this.OneWayBind(ViewModel, vm => vm.SuperFactor, v => v.SuperFactorContent.ViewModel);
                });
        }
    }
}
