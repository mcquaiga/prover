using Core;
using Core.Domain;
using Devices.Core.Interfaces;
using Module.Clients.Models;
using System;
using System.Collections.Generic;

namespace Module.Certificates.Certificates
{
    public class Certificate : AggregateRoot
    {
        #region Properties

        public string Apparatus { get; set; }

        public virtual Client Client { get; set; }

        public DateTimeOffset CreatedDateTime { get; set; }

        public virtual ICollection<IDevice> Devices { get; set; } = new List<IDevice>();

        public long Number { get; set; }

        public string TestedBy { get; set; }

        public VerificationType VerificationType { get; set; }

        public DateTimeOffset SealExpirationDate()
        {
            var period = 10; //Re-Verification
            if (VerificationType == VerificationType.New)
                period = 12;

            return CreatedDateTime.AddYears(period);
        }

        #endregion
    }
}