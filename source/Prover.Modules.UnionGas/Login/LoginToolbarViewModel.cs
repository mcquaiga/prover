using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Client.Desktop.Wpf.ViewModels;
using DcrWebService;
using Prover.Application.ViewModels;
using Prover.Shared.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Modules.UnionGas.Login
{
    public class LoginToolbarViewModel : ViewModelBase, IToolbarItem
    {
        public LoginToolbarViewModel(ILoginService<EmployeeDTO> loginService)
        {
            var loginService1 = loginService;

            LogIn = ReactiveCommand.CreateFromTask(async () => await loginService1.GetLoginDetails());

            LogOut = ReactiveCommand.CreateFromTask(async () =>
            {
                await Task.Run(() => loginService1.Logout());
                return false;
            });

            LogIn.Merge(LogOut)
                .Select(x => x ? loginService1.User?.EmployeeName : "")
                .ToPropertyEx(this, vm => vm.DisplayName);
        }

        public extern string DisplayName { [ObservableAsProperty] get; }

        public ReactiveCommand<Unit, bool> LogIn { get; }

        public ReactiveCommand<Unit, bool> LogOut { get; }
    }
}