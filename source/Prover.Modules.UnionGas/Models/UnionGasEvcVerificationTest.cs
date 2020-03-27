using System;
using System.Collections.Generic;
using System.Text;
using Devices.Core.Interfaces;
using Prover.Domain.EvcVerifications;

namespace Prover.Modules.UnionGas.Models
{
    public class UnionGasEvcVerification : EvcVerificationTest
    {
        public UnionGasEvcVerification(DeviceInstance device, string jobId, string employeeId) : base(device)
        {
            JobId = jobId;
            EmployeeId = employeeId;
        }

        public string JobId { get; set; }

        public string EmployeeId { get; set; }

        //public DateTime? ExportedDateTime { get; set; } = null;
    }
}
