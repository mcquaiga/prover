namespace UnionGas.MASA
{
    using Caliburn.Micro;
    using NLog;
    using Prover.Core.Login;
    using Prover.GUI.Screens;
    using System;
    using System.ServiceModel;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using UnionGas.MASA.DCRWebService;
    using UnionGas.MASA.Dialogs.LoginDialog;
    using UnionGas.MASA.Screens.Toolbars;

    /// <summary>
    /// Defines the <see cref="LoginService" />
    /// </summary>
    public class LoginService : ILoginService<EmployeeDTO>
    {
        #region Fields

        /// <summary>
        /// Defines the _log
        /// </summary>
        private readonly Logger _log = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Defines the _screenManager
        /// </summary>
        private readonly IScreenManager _screenManager;

        /// <summary>
        /// Defines the _webService
        /// </summary>
        private readonly DCRWebServiceSoap _webService;

        /// <summary>
        /// Defines the _eventAggregator
        /// </summary>
        private IEventAggregator _eventAggregator;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginService"/> class.
        /// </summary>
        /// <param name="screenManager">The screenManager<see cref="ScreenManager"/></param>
        /// <param name="eventAggregator">The eventAggregator<see cref="IEventAggregator"/></param>
        /// <param name="webService">The webService<see cref="DCRWebServiceSoap"/></param>
        public LoginService(ScreenManager screenManager, IEventAggregator eventAggregator, DCRWebServiceSoap webService)
        {
            _screenManager = screenManager;
            _eventAggregator = eventAggregator;
            _webService = webService;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether IsLoggedIn
        /// </summary>
        public bool IsLoggedIn => !string.IsNullOrEmpty(User?.Id);

        /// <summary>
        /// Gets the User
        /// </summary>
        public EmployeeDTO User { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// The GetLoginDetails
        /// </summary>
        /// <returns>The <see cref="Task{bool}"/></returns>
        public async Task<bool> GetLoginDetails()
        {
            var loginViewModel = _screenManager.ResolveViewModel<LoginDialogViewModel>();
            var result = _screenManager.ShowDialog(loginViewModel);
            var userId = result.HasValue && result.Value ? loginViewModel.EmployeeId : null;

            return await Login(userId);
        }

        /// <summary>
        /// The Login
        /// </summary>
        /// <param name="username">The username<see cref="string"/></param>
        /// <param name="password">The password<see cref="string"/></param>
        /// <returns>The <see cref="Task{bool}"/></returns>
        public async Task<bool> Login(string username, string password = null)
        {
            User = null;

            if (!string.IsNullOrEmpty(username))
            {
                var cts = new CancellationTokenSource(new TimeSpan(0, 0, 3));
                var ct = cts.Token;
                ct.ThrowIfCancellationRequested();

                _log.Debug($"Logging into MASA using Employee #{username} ...");
                try
                {
                    var employeeRequest = new GetEmployeeRequest(new GetEmployeeRequestBody(username));
                    var response =
                        await Task.Run(async () => await _webService.GetEmployeeAsync(employeeRequest), ct);

                    User = response.Body.GetEmployeeResult;
                }
                catch (EndpointNotFoundException)
                {
                    MessageBox.Show("Couldn't connect to web service.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    _log.Error("Endpoint could not be found.");
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                }
            }

            if (User?.Id != null)
                await _eventAggregator.PublishOnUIThreadAsync(new UserLoggedInEvent(UserLoggedInEvent.LogInState.LoggedIn));
            else
            {
                await _eventAggregator.PublishOnUIThreadAsync(new UserLoggedInEvent(UserLoggedInEvent.LogInState.LoggedOut));
            }

            return !string.IsNullOrEmpty(User?.Id);
        }

        /// <summary>
        /// The Logout
        /// </summary>
        /// <returns>The <see cref="bool"/></returns>
        public bool Logout()
        {
            User = null;
            _eventAggregator.PublishOnUIThreadAsync(new UserLoggedInEvent(UserLoggedInEvent.LogInState.LoggedOut));
            return true;
        }

        #endregion
    }
}
