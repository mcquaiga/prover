using System.Reactive.Disposables;
using Client.Desktop.Wpf.Extensions;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Verifications.Managers
{
    /// <summary>
    ///     Interaction logic for EditTestView.xaml
    /// </summary>
    public partial class RotaryTestManagerView
    {
        public RotaryTestManagerView()
        {
            InitializeComponent();

            var correctionsItemTemplate = FindResource("CorrectionsTestDataTemplate");
            var volumeContent = FindResource("RotaryVolumeContentControlTemplate");

            this.WhenActivated(d =>
            {
                //ViewModel.DeviceInteractions.RegisterDeviceInteractions();

                this.OneWayBind(ViewModel, vm => vm.TestViewModel, v => v.TestViewContent.ViewModel).DisposeWith(d);

                TestViewContent.Content.SetPropertyValue("CorrectionTestsItemTemplate", correctionsItemTemplate);
                TestViewContent.Content.SetPropertyValue("VolumeTestContentTemplate", volumeContent);

                this.CleanUpDefaults().DisposeWith(d);
            });
        }
    }
}