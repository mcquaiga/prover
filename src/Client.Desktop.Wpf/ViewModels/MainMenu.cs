using System;
using System.Reactive;
using System.Threading.Tasks;
using Client.Desktop.Wpf.Screens;
using Client.Desktop.Wpf.ViewModels.Clients;
using Client.Desktop.Wpf.ViewModels.Verifications;
using MaterialDesignThemes.Wpf;
using ReactiveUI;

namespace Client.Desktop.Wpf.ViewModels
{
    internal static class MainMenuItems
    {
        public static IMainMenuItem CertificatesMainMenu
            => new MainMenu("Certificates", PackIconKind.ClipboardText, order: 4,
                openFunc: s => s.ChangeView<ClientManagerViewModel>());

        public static IMainMenuItem ClientsMainMenu
            => new MainMenu("Clients", PackIconKind.User, order: 2,
                openFunc: s => s.ChangeView<ClientManagerViewModel>());

        public static IMainMenuItem VerificationsMainMenu
            => new MainMenu("New QA Test Run", PackIconKind.ClipboardCheck, order: 1,
                openFunc: s => s.ChangeView<NewTestViewModel>());

        //public static IMainMenuItem ExporterMainMenu(IScreenManager screenManager) 
        //    => new MainMenu(screenManager, "Export Test Runs ", PackIconKind.CloudUpload, s => s.ChangeView<ClientManagerViewModel>(), 4);
    }

    public class MainMenu : IMainMenuItem
    {
        public MainMenu(
            string menuTitle,
            PackIconKind menuIconKind,
            Func<IScreenManager, Task<IRoutableViewModel>> openFunc,
            int? order = null)
        {
            MenuIconKind = menuIconKind;
            MenuTitle = menuTitle;
            Order = order;

            OpenCommand = ReactiveCommand.CreateFromTask<IScreenManager, IRoutableViewModel>(openFunc.Invoke);
        }

        public PackIconKind MenuIconKind { get; }
        public string MenuTitle { get; }
        public virtual ReactiveCommand<IScreenManager, IRoutableViewModel> OpenCommand { get; }
        public int? Order { get; }
    }

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