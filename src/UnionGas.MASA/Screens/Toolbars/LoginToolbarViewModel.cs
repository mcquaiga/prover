using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Login;
using Prover.GUI.Common;
using Prover.GUI.Common.BackgroundWork;
using Prover.GUI.Common.Screens;
using Prover.GUI.Common.Screens.Toolbar;
using ReactiveUI;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Dialogs.LoginDialog;
using UnionGas.MASA.Screens.Toolbars.LoginToolbar;

namespace UnionGas.MASA.Screens.Toolbars
{
    public class LoginToolbarViewModel : ViewModelBase, IToolbarItem
    {
        private const string LoginViewContext = "Login";
        private const string LoggedInViewContext = "LoggedIn";
        private readonly ILoginService<EmployeeDTO> _loginService;
        
        public LoginToolbarViewModel(ScreenManager screenManager, IEventAggregator eventAggregator, ILoginService<EmployeeDTO> loginService) : base(screenManager, eventAggregator)
        {
            _loginService = loginService;

            ViewContext = LoginViewContext;
        }

        public string ViewContext { get; set; }

        public string Username => _loginService.User.EmployeeName;

        public async Task LoginButton()
        {
            var loginViewModel = ScreenManager.ResolveViewModel<LoginDialogViewModel>();
            var result = ScreenManager.ShowDialog(loginViewModel);
            var userId = result.HasValue && result.Value ? loginViewModel.EmployeeId : null;

            if (userId != null)
            {
                var success = await _loginService.Login(userId);
                if (success) ChangeContext(LoggedInViewContext);
            }
        }

        public async Task LogoutButton()
        {
            await Task.Run(() => _loginService.Logout());
            ChangeContext(LoginViewContext);
        }

        private void ChangeContext(string contextName)
        {
            ViewContext = contextName;
            NotifyOfPropertyChange(() => ViewContext);
        }
    }
}
