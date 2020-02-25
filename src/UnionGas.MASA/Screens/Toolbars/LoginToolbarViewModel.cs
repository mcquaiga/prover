using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.Core.Login;
using Prover.GUI.Screens;
using UnionGas.MASA.DCRWebService;

namespace UnionGas.MASA.Screens.Toolbars
{
    public class LoginToolbarViewModel : ViewModelBase, IToolbarItem, IHandle<UserLoggedInEvent>, IDisplayOnStartup
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

        public string Username => _loginService.User?.EmployeeName;

        public async Task LoginButton()
        {
            ChangeContext(WaitingForLogInViewContext);
            await _loginService.GetLoginDetails();                         
        }

        public async Task LogoutButton()
        {
            await _loginService.Logout();     
        }

        private void ChangeContext(string contextName)
        {
            ViewContext = contextName;
            
            NotifyOfPropertyChange(() => ViewContext);
            NotifyOfPropertyChange(() => Username);
        }

        public void Handle(UserLoggedInEvent message)
        {
            if (message.LoginStatus == UserLoggedInEvent.LogInState.LoggedIn)
                ChangeContext(LoggedInViewContext);

            if (message.LoginStatus == UserLoggedInEvent.LogInState.LoggedOut)
                ChangeContext(LoginViewContext);
        }

        public async Task Show()
        {
            await _loginService.GetLoginDetails();
        }
    }
}
