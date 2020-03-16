using System.Globalization;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using Client.Desktop.Wpf.Communications;
using Devices.Core.Items;
using Prover.Application.Interfaces;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Verifications.Dialogs
{
    /// <summary>
    ///     Interaction logic for LiveReadDialogView.xaml
    /// </summary>
    public partial class LiveReadDialogView : ReactiveUserControl<LiveReadCoordinator>
    {
        public LiveReadDialogView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                var tempItem = ViewModel.DeviceType.GetLiveTemperatureItem();
                if (tempItem != null && ViewModel.ItemTargets.ContainsKey(tempItem))
                {
                    TemperatureTargetValueTextBlock.Text = ViewModel.ItemTargets[tempItem].ToString(CultureInfo.CurrentCulture);

                    ViewModel.LiveReadUpdates
                        .Where(i => i.Metadata.IsLiveReadTemperature == true)
                        .Select(i => i.DecimalValue()?.ToString() ?? "")
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .BindTo(this, view => view.TemperatureValueTextBlock.Text)
                        .DisposeWith(d);
                }
                else
                {
                    TemperatureStackPanelControl.Visibility = Visibility.Collapsed;
                }

                var pressureItem = ViewModel.DeviceType.GetLivePressureItem();
                if (pressureItem != null && ViewModel.ItemTargets.ContainsKey(pressureItem))
                {
                    PressureTargetValueTextBlock.Text = ViewModel.ItemTargets[pressureItem].ToString(CultureInfo.CurrentCulture);

                    ViewModel.LiveReadUpdates
                        .Where(i => i.Metadata.IsLiveReadPressure == true)
                        .Select(i => i.DecimalValue()?.ToString() ?? "")
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .BindTo(this, view => view.PressureValueTextBlock.Text)
                        .DisposeWith(d);
                }
                else
                {
                    PressureStackPanelControl.Visibility = Visibility.Collapsed;
                }
  
                //this.OneWayBind(ViewModel, vm => vm.LiveReadUpdates., v => v.PressureItemValueControl.Value, value => value);
                
            });
        }
        
    }
}