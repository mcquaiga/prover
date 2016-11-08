using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Login;
using Prover.GUI.Common;
using Prover.GUI.Common.Screens;
using Prover.GUI.Common.Screens.Toolbar;
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
            _loginService = loginService as LoginService;

            ViewContext = LoginViewContext;
        }

        public string ViewContext { get; set; }

        public string Username => _loginService.User.EmployeeName;

        public async Task LoginButton()
        {
            var loginViewModel = ScreenManager.ResolveViewModel<LoginDialogViewModel>();
            var result = ScreenManager.ShowDialog(loginViewModel);
            var userId = result.HasValue && result.Value ? loginViewModel.EmployeeId : null;

            var isSuccess = await _loginService.Login(userId);

            if (isSuccess) ChangeContext(LoggedInViewContext);
        }

        private void ChangeContext(string contextName)
        {
            ViewContext = contextName;
            NotifyOfPropertyChange(() => ViewContext);
        }
    }
}
