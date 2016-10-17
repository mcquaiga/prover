using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using Prover.Core.Login;
using Prover.GUI.Common;
using UnionGas.MASA.DCRWebService;
using UnionGas.MASA.Dialogs.LoginDialog;

namespace UnionGas.MASA
{
    public class LoginService : ILoginService
    {
        private IEventAggregator _eventAggregator;
        private DCRWebServiceSoap _webService;
        private IUnityContainer _container;
        public EmployeeDTO EmployeeDto { get; set; }

        public LoginService(IUnityContainer container, IEventAggregator eventAggregator, DCRWebServiceSoap webService)
        {
            _container = container;
            _eventAggregator = eventAggregator;
            _webService = webService;
        }

        public async Task<object> GetLoginInfoFromUser()
        {
            var loginInput = new LoginDialogViewModel();
            ScreenManager.ShowDialog(_container, loginInput);

            return loginInput.EmployeeId;
        }

        public async Task<object> Login(string username = null, string password = null)
        {
            if (string.IsNullOrEmpty(username))
                username = (string)await GetLoginInfoFromUser();

            try
            {
                var employeeRequest = new GetEmployeeRequest(new GetEmployeeRequestBody(username));
                var response = await _webService.GetEmployeeAsync(employeeRequest);

                EmployeeDto = response.Body.GetEmployeeResult;

                return response.Body.GetEmployeeResult?.Id;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

    }
}
