using Prover.Application.Interactions;
using Prover.UI.Desktop.Extensions;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Prover.UI.Desktop.Views.Verifications
{
    /// <summary>
    ///     Interaction logic for EditTestView.xaml
    /// </summary>
    public partial class TestManagerView
    {
        public TestManagerView()
        {
            InitializeComponent();
            RegisterInteractionHandlers();

            var correctionsItemTemplate = FindResource("CorrectionsTestDataTemplate");
            var volumeContent = FindResource("RotaryVolumeContentControlTemplate");

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.TestViewModel, v => v.TestViewContent.ViewModel).DisposeWith(d);

                this.BindCommand(ViewModel,
                vm => vm.SaveCommand,
                v => v.ActionSnackbarMessage.ActionCommand).DisposeWith(d);


                TestViewContent.Content.SetPropertyValue("CorrectionTestsItemTemplate", correctionsItemTemplate);
                TestViewContent.Content.SetPropertyValue("VolumeTestContentTemplate", volumeContent);

                this.CleanUpDefaults().DisposeWith(d);
            });


        }

        private void RegisterInteractionHandlers()
        {
            Notifications.ActionMessage.RegisterHandler(context =>
            {
                ActionSnackbar.Message.Content = context.Input;
                ActionSnackbar.IsActive = true;

                Observable.Timer(TimeSpan.FromSeconds(30))
                          .ObserveOn(RxApp.MainThreadScheduler)
                          .Subscribe(_ =>
                          {
                              ActionSnackbar.IsActive = false;
                              ActionSnackbar.Message.Content = string.Empty;
                          });

                context.SetOutput(Unit.Default);
            });
        }
    }
}