using System;
using System.Reactive;
using System.Threading.Tasks;
using Client.Wpf.Screens;
using Client.Wpf.ViewModels.Clients;
using Client.Wpf.ViewModels.Verifications;
using MaterialDesignThemes.Wpf;
using ReactiveUI;

namespace Client.Wpf.ViewModels
{
    public static class MainMenuItems
    {
        public static IMainMenuItem VerificationsMainMenu(IScreenManager screenManager) 
            => new MainMenu(screenManager, "New QA Test Run", PackIconKind.ClipboardCheck, s => s.ChangeView<NewTestViewModel>(), 1);
        public static IMainMenuItem ClientsMainMenu(IScreenManager screenManager) 
            => new MainMenu(screenManager, "Clients", PackIconKind.User, s => s.ChangeView<ClientManagerViewModel>(), 2);

        public static IMainMenuItem CertificatesMainMenu(IScreenManager screenManager) 
            => new MainMenu(screenManager, "Certificates", PackIconKind.ClipboardText, s => s.ChangeView<ClientManagerViewModel>(), 4);


        private class MainMenu : IMainMenuItem
        {
            protected readonly IScreenManager ScreenManager;

            protected MainMenu() { }

            protected MainMenu(IScreenManager screenManager, PackIconKind menuIconKind, string menuTitle, int order)
            {
                ScreenManager = screenManager;
                MenuIconKind = menuIconKind;
                MenuTitle = menuTitle;
                Order = order;
            }

            public MainMenu(IScreenManager screenManager, 
                string menuTitle, 
                PackIconKind menuIconKind, 
                Func<IScreenManager, Task<IRoutableViewModel>> openFunc, int? order = null)
            {
                ScreenManager = screenManager;
                MenuIconKind = menuIconKind;
                MenuTitle = menuTitle;
                Order = order;

                OpenCommand =
                    ReactiveCommand.CreateFromTask<Unit, IRoutableViewModel>(_ => openFunc.Invoke(ScreenManager));
            }

            public PackIconKind MenuIconKind { get; }
            public string MenuTitle { get; }
            public virtual ReactiveCommand<Unit, IRoutableViewModel> OpenCommand { get; }
            public int? Order { get; }
        }
    }


    //public class CertificateManagerModule : MainMenu
    //{
    //    public override ReactiveCommand<Unit, IRoutableViewModel> OpenCommand { get; }

    //    public CertificateManagerModule(IScreenManager screenManager) 
    //        : base(screenManager, 
    //            PackIconKind.ClipboardText, 
    //            "Certificates", 
    //            3)
    //    {
    //        OpenCommand = ReactiveCommand.CreateFromTask<Unit, IRoutableViewModel>(_ => screenManager.ChangeView<ClientManagerViewModel>());
    //    }
    //}
}