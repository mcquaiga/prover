using System.Collections.Generic;

namespace Prover.Application.Settings
{

    public class CertificateSettings
    {
        public string McRegistrationNumber { get; set; } = string.Empty;
        public List<MeasurementApparatus> MeasurementApparatuses { get; set; } = new List<MeasurementApparatus>();

        public class MeasurementApparatus
        {            
            public string Description { get; set; }
            public string SerialNumbers { get; set; }
        }
    }
}