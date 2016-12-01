using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using NLog;
using NLog.Fluent;
using Prover.Core.Login;
using Prover.GUI.Common;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Dialogs.LoginDialog;

namespace UnionGas.MASA
{
    public class LoginService : ILoginService<EmployeeDTO>
    {
        private readonly Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private readonly IScreenManager _screenManager;
        private readonly DCRWebServiceSoap _webService;
        private IEventAggregator _eventAggregator;

        public LoginService(ScreenManager screenManager, IEventAggregator eventAggregator, DCRWebServiceSoap webService)
        {
            _screenManager = screenManager;
            _eventAggregator = eventAggregator;
            _webService = webService;
        }

        public EmployeeDTO User { get; private set; }

        public async Task<bool> Login(string username, string password = null)
        {
            User = null;

            _log.Debug($"Logging into MASA using Employee Number {username} ...");
            try
            {
                var employeeRequest = new GetEmployeeRequest(new GetEmployeeRequestBody(username));
                var response = await _webService.GetEmployeeAsync(employeeRequest);

                User = response.Body.GetEmployeeResult;
            }
            catch (EndpointNotFoundException)
            {
                MessageBox.Show("Couldn't connect to web service.");
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            return User.Id != null;
        }
    }
}