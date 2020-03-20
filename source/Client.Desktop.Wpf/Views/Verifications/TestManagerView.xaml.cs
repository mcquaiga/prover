using System.Reactive.Disposables;
using Client.Desktop.Wpf.Extensions;
using Client.Desktop.Wpf.ViewModels.Verifications;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Verifications
{
    /// <summary>
    ///     Interaction logic for EditTestView.xaml
    /// </summary>
    public partial class TestManagerView : ReactiveUserControl<TestManager>
    {
        public TestManagerView()
        {
            InitializeComponent();

            var correctionsItemTemplate = FindResource("CorrectionsTestDataTemplate");
            var volumeContent = FindResource("RotaryVolumeContentControlTemplate");

            this.WhenActivated(d =>
            {
                //ViewModel.DeviceInteractions.RegisterDeviceInteractions();

                this.OneWayBind(ViewModel, vm => vm.TestViewModel, v => v.TestViewContent.ViewModel).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.PrintTestReport, v => v.PrintButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.SubmitTest, v => v.SubmitTestButton).DisposeWith(d);
                
                TestViewContent.Content.SetPropertyValue("CorrectionTestsItemTemplate", correctionsItemTemplate);
                TestViewContent.Content.SetPropertyValue("VolumeTestContentTemplate", volumeContent);

                this.CleanUpDefaults().DisposeWith(d);
            });
        }
    }
}