using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Client.Desktop.Wpf.ViewModels;
using Prover.Application.Interactions;
using Prover.Application.ViewModels;
using Prover.Modules.UnionGas.DcrWebService;
using Prover.Shared.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Modules.UnionGas.Login
{
    public class LoginToolbarViewModel : ViewModelBase, IToolbarItem
    {
        public LoginToolbarViewModel(ILoginService<EmployeeDTO> loginService)
        {
            LoginService = loginService;
            LogIn = ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await MessageInteractions.GetInputString.Handle("Employee number");
                
                if (string.IsNullOrEmpty(result)) 
                    return false;

                return await loginService.Login(result);
            });

            LogOut = ReactiveCommand.CreateFromTask(loginService.Logout);

            LogIn.Merge(LogOut)
                .Select(x => x ? loginService.User?.EmployeeName : "")
                .ToPropertyEx(this, vm => vm.DisplayName);
        }

        public ILoginService<EmployeeDTO> LoginService { get; }

        public extern string DisplayName { [ObservableAsProperty] get; }

        public ReactiveCommand<Unit, bool> LogIn { get; }

        public ReactiveCommand<Unit, bool> LogOut { get; }
    }
}