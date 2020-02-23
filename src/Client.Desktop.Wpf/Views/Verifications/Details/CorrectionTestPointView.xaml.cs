using System.Linq;
using Application.ViewModels;
using Application.ViewModels.Corrections;
using Devices.Core.Items.ItemGroups;
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

                    this.OneWayBind(ViewModel, vm => vm.TestsCollection, v => v.TestItems.ItemsSource, 
                        tests => tests.Where(t =>
                        {
                            var baseType = t.GetType().BaseType;

                            if (baseType != null && (baseType.IsGenericType || baseType.IsGenericTypeDefinition))
                            {
                                var success = baseType.GetGenericTypeDefinition() == typeof(CorrectionTestViewModel<>);
                                return success;
                            }
                            return false;
                        }));
                    
                    //this.OneWayBind(ViewModel, vm => vm.Pressure, v => v.PressureContent.ViewModel);
                    //this.OneWayBind(ViewModel, vm => vm.Temperature, v => v.TemperatureContent.ViewModel);
                    //this.OneWayBind(ViewModel, vm => vm.SuperFactor, v => v.SuperFactorContent.ViewModel);
                });
        }
    }
}
