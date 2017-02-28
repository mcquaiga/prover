using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prover.Core.Models.Instruments;

namespace Prover.Modules.Certificates.Models
{
    public class CertificateInstrument : Instrument
    {
        public virtual Certificate Certificate { get; set; }
    }
}
