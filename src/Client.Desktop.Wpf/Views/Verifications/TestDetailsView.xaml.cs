using Client.Wpf.ViewModels.Verifications;
using ReactiveUI;
using System.Reactive.Disposables;

namespace Client.Wpf.Views.Verifications
{
    /// <summary>
    /// Interaction logic for EditTestView.xaml
    /// </summary>
    public partial class TestDetailsView : ReactiveUserControl<TestDetailsViewModel>
    {
        public TestDetailsView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
                {
                    this.OneWayBind(ViewModel, vm => vm.EvcVerification.Tests, v => v.TestPointItems.ItemsSource).DisposeWith(d);

                    this.OneWayBind(ViewModel, vm => vm.EvcVerification, v => v.SiteInfoContent.ViewModel).DisposeWith(d);
                    this.OneWayBind(ViewModel, vm => vm.EvcVerification.VolumeTest, v => v.VolumeContentHost.ViewModel).DisposeWith(d);

                    this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);
                    
                });
        }
    }
}
