using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
using Client.Desktop.Wpf.Communications;
using DynamicData;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Verifications.Dialogs
{
    /// <summary>
    /// Interaction logic for LiveReadDialogView.xaml
    /// </summary>
    public partial class LiveReadDialogView : ReactiveUserControl<DeviceSessionManager>
    {
        public LiveReadDialogView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.BindCommand(ViewModel, x => x.RequestCancellation, v => v.CancelButton).DisposeWith(d);

                //this.OneWayBind(ViewModel, vm => vm.LiveReadUpdates., v => v.PressureItemValueControl.Value, value => value);
                ViewModel.LiveReadUpdates.Connect()
                    .Filter(i => i.Metadata.IsLiveReadTemperature == true)
                    //.Select(i => i.DecimalValue()?.ToString() ?? "")
                    .BindTo(this, view => view.TempItemValueControl.Value)
                    .DisposeWith(d);

                ViewModel.LiveUpdates
                    .Connect()
                    .Select(x => x.)
                    .BindTo(this, view => view.TempItemValueControl.Value);
            });

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            //ViewModel
        }
    }
}
