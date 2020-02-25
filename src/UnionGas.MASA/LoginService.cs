using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using NLog;
using Prover.Core.Login;
using Prover.GUI.Screens;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Dialogs.LoginDialog;
using UnionGas.MASA.Screens.Toolbars;
using LogManager = NLog.LogManager;

namespace UnionGas.MASA
{
    /// <summary>
    ///     Defines the <see cref="LoginService" />
    /// </summary>
    public class LoginService : ILoginService<EmployeeDTO>
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoginService" /> class.
        /// </summary>
        /// <param name="screenManager">The screenManager<see cref="ScreenManager" /></param>
        /// <param name="eventAggregator">The eventAggregator<see cref="IEventAggregator" /></param>
        /// <param name="webService">The webService<see cref="DCRWebServiceSoap" /></param>
        public LoginService(ScreenManager screenManager, IEventAggregator eventAggregator,
            DCRWebServiceCommunicator webService)
        {
            _screenManager = screenManager;
            _eventAggregator = eventAggregator;
            _webService = webService;
        }

        #endregion

        #region IDisplayOnStartup Members

        

        #endregion

        #region Fields

        private readonly EmployeeDTO _employeeTest = new EmployeeDTO
        {
            EmployeeName = "Adam",
            EmployeeNbr = "123456",
            Id = "99999"
        };

        /// <summary>
        ///     Defines the _log
        /// </summary>
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Defines the _screenManager
        /// </summary>
        private readonly IScreenManager _screenManager;

        /// <summary>
        ///     Defines the _webService
        /// </summary>
        private readonly DCRWebServiceCommunicator _webService;

        /// <summary>
        ///     Defines the _eventAggregator
        /// </summary>
        private readonly IEventAggregator _eventAggregator;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a value indicating whether IsLoggedIn
        /// </summary>
        public bool IsLoggedIn => !string.IsNullOrEmpty(User?.Id);

        /// <summary>
        ///     Gets the User
        /// </summary>
        public EmployeeDTO User { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///     The GetLoginDetails
        /// </summary>
        /// <returns>The <see cref="Task{bool}" /></returns>
        public async Task<bool> GetLoginDetails()
        {
            var loginViewModel = _screenManager.ResolveViewModel<LoginDialogViewModel>();
            var result = _screenManager.ShowDialog(loginViewModel);
            var userId = result.HasValue && result.Value ? loginViewModel.EmployeeId : null;

            return await Login(userId);
        }

        /// <summary>
        ///     The Login
        /// </summary>
        /// <param name="username">The username<see cref="string" /></param>
        /// <param name="password">The password<see cref="string" /></param>
        /// <returns>The <see cref="Task{bool}" /></returns>
        public async Task<bool> Login(string username, string password = null)
        {
            User = null;

            try
            {
                if (!string.IsNullOrEmpty(username) && _employeeTest.EmployeeNbr != username)
                    User = await _webService.GetEmployee(username);
                else if (_employeeTest.EmployeeNbr == username) User = _employeeTest;
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                if (User?.Id != null)
                    await _eventAggregator.PublishOnUIThreadAsync(
                        new UserLoggedInEvent(UserLoggedInEvent.LogInState.LoggedIn));
                else
                    await _eventAggregator.PublishOnUIThreadAsync(
                        new UserLoggedInEvent(UserLoggedInEvent.LogInState.LoggedOut));
            }

            return !string.IsNullOrEmpty(User?.Id);
        }

        /// <summary>
        ///     The Logout
        /// </summary>
        /// <returns>The <see cref="bool" /></returns>
        public async Task<bool> Logout()
        {
            User = null;
            await _eventAggregator.PublishOnUIThreadAsync(new UserLoggedInEvent(UserLoggedInEvent.LogInState.LoggedOut));
            return true;
        }

        #endregion
    }
}