using System.Reactive;
using Client.Wpf.Screens;
using MaterialDesignThemes.Wpf;
using ReactiveUI;

namespace Client.Wpf.ViewModels.Verifications
{
    public class VerificationsMainMenuItem : MainMenuItem
    {
        public override ReactiveCommand<Unit, IRoutableViewModel> OpenCommand { get; }

        public VerificationsMainMenuItem(IScreenManager screenManager) 
            : base(screenManager, 
                PackIconKind.ClipboardCheck, 
                "New QA Test Run", 
                1)
        {
            OpenCommand = ReactiveCommand.CreateFromTask<Unit, IRoutableViewModel>(_ => screenManager.ChangeView<NewTestViewModel>());
        }
    }
}