using System.Reactive.Disposables;
using Client.Wpf.ViewModels;
using Client.Wpf.ViewModels.Verifications;
using ReactiveUI;

namespace Client.Wpf.Views.Verifications
{
    /// <summary>
    /// Interaction logic for VerificationTest.xaml
    /// </summary>
    public partial class NewTestView : ReactiveUserControl<NewTestViewModel>
    {
        public NewTestView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                DataContext = ViewModel;

                this.BindCommand(ViewModel, vm => vm.StartTestCommand, v => v.StartTestButton).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.DeviceTypes, v => v.DeviceTypes.ItemsSource);

            });
        }
    }
}
