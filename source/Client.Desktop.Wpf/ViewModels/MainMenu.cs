using System.Reactive;
using System.Reactive.Linq;
using Client.Desktop.Wpf.ViewModels.Verifications;
using MaterialDesignThemes.Wpf;
using Prover.Application.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.ViewModels
{
    public class TestManagerFactoryCoordinator
    {
        public NewTestRunViewModel TestRunViewModel { get; }
        public QaTestRunViewModel QaTestRunViewModel { get; }

        public TestManagerFactoryCoordinator(IScreenManager screenManager, NewTestRunViewModel testRunViewModel,
            QaTestRunViewModel qaTestRunViewModel)
        {
            TestRunViewModel = testRunViewModel;
            QaTestRunViewModel = qaTestRunViewModel;

            //this.TestRunViewModel
            //    .StartTestCommand
            //    .InvokeCommand(ReactiveCommand.CreateFromTask<IRoutableViewModel>(screenManager.ChangeView));
            
            StartTest = ReactiveCommand.CreateFromObservable(() =>
            {
                screenManager.DialogManager.ShowViewModel(testRunViewModel);
                return Observable.Return(Unit.Default);
            });
        }

        public ReactiveCommand<Unit, Unit> StartTest { get; }
    }

    public class VerificationsMainMenu : IMainMenuItem
    {
        public VerificationsMainMenu(IScreenManager screenManager, TestManagerFactoryCoordinator managerFactory)
        {
            OpenCommand = ReactiveCommand.CreateFromObservable(() =>
            {
                screenManager.DialogManager.Show<NewTestRunViewModel>();
                return Observable.Return(Unit.Default);
            });
        }

        public PackIconKind MenuIconKind { get; } = PackIconKind.ClipboardCheck;
        public string MenuTitle { get; } = "New QA Test Run";
        public int? Order { get; } = 1;

        public ReactiveCommand<Unit, Unit> OpenCommand { get; }
    }

    public interface IMainMenuItem
    {
        PackIconKind MenuIconKind { get; }
        string MenuTitle { get; }

        ReactiveCommand<Unit, Unit> OpenCommand { get; }

        int? Order { get; }
    }

    //public class MainMenuItem : IMainMenuItem
    //{
    //    protected MainMenuItem()
    //    {

    //    }

    //    public MainMenuItem(
    //        string menuTitle,
    //        PackIconKind menuIconKind,
    //        Func<IScreenManager, Task<IRoutableViewModel>> openFunc,
    //        int? order = null)
    //    {
    //        MenuIconKind = menuIconKind;
    //        MenuTitle = menuTitle;
    //        Order = order;

    //        OpenCommand = ReactiveCommand.CreateFromTask<Unit, Unit>(openFunc.Invoke);
    //    }

    //    public PackIconKind MenuIconKind { get; }
    //    public string MenuTitle { get; }
    //    public ReactiveCommand<Unit, Unit> OpenCommand { get; }
    //    public int? Order { get; }
    //}

    //public class CertificateManagerModule : MainMenu
    //{
    //    public CertificateManagerModule(IScreenManager screenManager)
    //        : base(screenManager,
    //            PackIconKind.ClipboardText,
    //            "Certificates",
    //            3)
    //    {
    //        OpenCommand =
    //            ReactiveCommand.CreateFromTask<Unit, IRoutableViewModel>(_ =>
    //                screenManager.ChangeView<ClientManagerViewModel>());
    //    }

    //    public override ReactiveCommand<Unit, IRoutableViewModel> OpenCommand { get; }
    //}
}