using MaterialDesignThemes.Wpf;
using Prover.Application.Interfaces;
using Prover.UI.Desktop.Extensions;
using Prover.UI.Desktop.ViewModels;
using ReactiveUI;
using System.Reactive.Disposables;

namespace Prover.UI.Desktop.Views.Verifications
{
	/// <summary>
	///     Interaction logic for EditTestView.xaml
	/// </summary>
	public partial class TestManagerView
	{
		public TestManagerView(IToolbarManager toolbarManager)
		{
			InitializeComponent();
			RegisterInteractionHandlers();

			var correctionsItemTemplate = FindResource("CorrectionsTestDataTemplate");
			var volumeContent = FindResource("RotaryVolumeContentControlTemplate");

			this.WhenActivated(d =>
			{
				this.OneWayBind(ViewModel, vm => vm.TestViewModel, v => v.TestViewContent.ViewModel).DisposeWith(d);

				//this.BindCommand(ViewModel,
				//vm => vm.SaveCommand,
				//v => v.ActionSnackbarMessage.ActionCommand).DisposeWith(d);

				toolbarManager.AddToolbarItem(ViewModel.SaveCommand, PackIconKind.ContentSave).DisposeWith(d);
				toolbarManager.AddToolbarItem(ViewModel.SubmitTest, PackIconKind.Send).DisposeWith(d);
				toolbarManager.AddToolbarItem(ViewModel.PrintTestReport, PackIconKind.PrintPreview).DisposeWith(d);

				TestViewContent.Content.SetPropertyValue("CorrectionTestsItemTemplate", correctionsItemTemplate);
				TestViewContent.Content.SetPropertyValue("VolumeTestContentTemplate", volumeContent);

				this.CleanUpDefaults().DisposeWith(d);
			});
		}

		private void RegisterInteractionHandlers()
		{
			//Notifications.ActionMessage.RegisterHandler(context =>
			//{
			//    ActionSnackbar.Message.Content = context.Input;
			//    ActionSnackbar.IsActive = true;

			//    Observable.Timer(TimeSpan.FromSeconds(30))
			//              .ObserveOn(RxApp.MainThreadScheduler)
			//              .Subscribe(_ =>
			//              {
			//                  ActionSnackbar.IsActive = false;
			//                  ActionSnackbar.Message.Content = string.Empty;
			//              });

			//    context.SetOutput(Unit.Default);
			//});
		}
	}
}