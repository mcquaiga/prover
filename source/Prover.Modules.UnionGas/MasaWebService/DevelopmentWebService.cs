﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Prover.Application.Interfaces;
using Prover.Modules.UnionGas.DcrWebService;

namespace Prover.Modules.UnionGas.MasaWebService
{
    public class DevelopmentWebService :
        IUserService<EmployeeDTO>,
        IMeterService<MeterDTO>,
        IExportService<QARunEvcTestResult>
    {
        private readonly ICollection<EmployeeDTO> _users = new List<EmployeeDTO>
        {
            new EmployeeDTO {EmployeeName = "Adam McQuaig", EmployeeNbr = "123", Id = "1"},
            new EmployeeDTO {EmployeeName = "Tony", EmployeeNbr = "1234", Id = "2"},
            new EmployeeDTO {EmployeeName = "Glendon Evans", EmployeeNbr = "12345", Id = "3"},
            new EmployeeDTO {EmployeeName = "Kyle Johnson", EmployeeNbr = "123456", Id = "4"}
        };

        private readonly IDeviceSessionManager _deviceSession;

        public DevelopmentWebService(IDeviceSessionManager deviceSession)
        {
            _deviceSession = deviceSession;
        }

        public async Task<EmployeeDTO> GetUser(string employeeNumber)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return _users.FirstOrDefault(u => u.EmployeeNbr == employeeNumber);
        }

        public async Task<MeterDTO> FindMeterByInventoryNumber(string inventoryNumber)
        {
            await Task.CompletedTask;
            var meterNumber = RandomNumberGenerator.GetInt32(0, 2);

            if (meterNumber.ToString() == inventoryNumber)
            {
                return new MeterDTO()
                {
                    InventoryCode = inventoryNumber,
                    JobNumber = RandomNumberGenerator.GetInt32(1, 10000),
                    SerialNumber = _deviceSession?.Device.Items.SiteInfo.SerialNumber ?? "1234"
                };
            }

            return null;
        }

        public async Task<IList<MeterDTO>> GetOutstandingMeterTestsByJobNumber(int jobNumber)
        {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public async Task<bool> SubmitQaTestRunResults(IEnumerable<QARunEvcTestResult> evcQaRuns)
        {
            await Task.CompletedTask;
            return true;
        }
    }
}