using Prover.Application.Interactions;
using Prover.Application.ViewModels;
using Prover.Modules.UnionGas.Models;
using Prover.Shared.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Prover.Modules.UnionGas.Login
{
    public class LoginToolbarViewModel : ViewModelBase, IToolbarItem
    {
        public LoginToolbarViewModel(ILoginService<Employee> loginService)
        {
            LoginService = loginService;
            LogIn = ReactiveCommand.CreateFromTask(async () =>
            {
                var username = await loginService.GetLoginDetails();
                return await loginService.Login(username);
            });
            LogIn.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(async ex => await Exceptions.Error.Handle($"An error occured signing on. {Environment.NewLine} {ex.Message}"))
                .Subscribe();

            LogIn.Where(x => !x)
                .Do(async _ => await Notifications.SnackBarMessage.Handle("Employee not found"))
                .Subscribe();

            LogOut = ReactiveCommand.CreateFromTask(loginService.Logout);

            loginService.LoggedIn
                .Select(x => x ? loginService.User?.UserName : "")
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToPropertyEx(this, vm => vm.DisplayName);
        }

        public ILoginService<Employee> LoginService { get; }

        public extern string DisplayName { [ObservableAsProperty] get; }

        public ReactiveCommand<Unit, bool> LogIn { get; }

        public ReactiveCommand<Unit, Unit> LogOut { get; }
    }
}