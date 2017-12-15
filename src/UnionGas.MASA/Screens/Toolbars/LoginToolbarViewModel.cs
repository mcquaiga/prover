using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Login;
using Prover.GUI.Screens;
using Prover.GUI.Screens.Toolbar;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Dialogs.LoginDialog;

namespace UnionGas.MASA.Screens.Toolbars
{
    public class LoginToolbarViewModel : ViewModelBase, IToolbarItem
    {
        private const string LoginViewContext = "Login";
        private const string LoggedInViewContext = "LoggedIn";
        private const string WaitingForLogInViewContext = "WaitingForLogin";
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
                ChangeContext(WaitingForLogInViewContext);
                var success = await _loginService.Login(userId);
                ChangeContext(success ? LoggedInViewContext : LoginViewContext);
            }

            loginViewModel = null;
            result = null;
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
            NotifyOfPropertyChange(() => Username);
        }
    }
}
