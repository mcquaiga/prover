using System;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Prover.Core.Login;
using Prover.GUI.Common;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Dialogs.LoginDialog;

namespace UnionGas.MASA
{
    public class LoginService : ILoginService
    {
        private readonly IScreenManager _screenManager;
        private readonly DCRWebServiceSoap _webService;
        private IEventAggregator _eventAggregator;

        public LoginService(IScreenManager screenManager, IEventAggregator eventAggregator, DCRWebServiceSoap webService)
        {
            _screenManager = screenManager;
            _eventAggregator = eventAggregator;
            _webService = webService;
        }

        public EmployeeDTO EmployeeDto { get; private set; }

        public async Task<object> Login(string username = null, string password = null)
        {
            if (string.IsNullOrEmpty(username))
                username = (string) await GetLoginInfoFromUser();

            try
            {
                var employeeRequest = new GetEmployeeRequest(new GetEmployeeRequestBody(username));
                var response = await _webService.GetEmployeeAsync(employeeRequest);

                EmployeeDto = response.Body.GetEmployeeResult;

                if (EmployeeDto != null)
                    MessageBox.Show($"Logged in as {EmployeeDto.EmployeeName}.");

                return response.Body.GetEmployeeResult?.Id;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<object> GetLoginInfoFromUser()
        {
            return await Task.Run(() =>
            {
                var loginViewModel = _screenManager.ResolveViewModel<LoginDialogViewModel>();
                var result = _screenManager.ShowDialog(loginViewModel);

                return result.HasValue && result.Value ? loginViewModel.EmployeeId : null;
            });
        }
    }
}