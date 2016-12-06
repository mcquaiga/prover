﻿using System;
using System.ServiceModel;
using System.Threading;
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

        public bool Logout()
        {
            User = null;

            return true;
        }
        
        public EmployeeDTO User { get; private set; }

        public async Task<bool> Login(string username, string password = null)
        {
            var cts = new CancellationTokenSource(new TimeSpan(0, 0,3));
            var ct = cts.Token;
            ct.ThrowIfCancellationRequested();

            User = null;

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
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            return User.Id != null;
        }


    }
}