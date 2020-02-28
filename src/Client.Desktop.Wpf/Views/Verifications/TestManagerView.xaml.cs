using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Client.Desktop.Wpf.Communications;
using Client.Desktop.Wpf.Extensions;
using Client.Desktop.Wpf.ViewModels.Verifications;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Verifications
{
    /// <summary>
    /// Interaction logic for EditTestView.xaml
    /// </summary>
    public partial class TestManagerView : ReactiveUserControl<TestManagerViewModel>
    {
        public TestManagerView()
        {
            InitializeComponent();

            var correctionsItemTemplate = FindResource("CorrectionsTestDataTemplate");
            var volumeContent = FindResource("RotaryVolumeContentControlTemplate");

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.TestViewModel, v => v.TestViewContent.ViewModel).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.PrintTestReport, v => v.PrintButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.CompleteTest, v => v.SubmitTestButton).DisposeWith(d);
                
                
                TestViewContent.Content.SetPropertyValue("CorrectionTestsItemTemplate", correctionsItemTemplate);
                TestViewContent.Content.SetPropertyValue("VolumeTestContentTemplate", volumeContent);

                this.CleanUpDefaults().DisposeWith(d);
               
            });

        }

    }

}
