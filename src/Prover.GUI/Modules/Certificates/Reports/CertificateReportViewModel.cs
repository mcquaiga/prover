using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro.ReactiveUI;
using Prover.Core.Models.Certificates;
using Prover.GUI.Modules.Certificates.Screens;

namespace Prover.GUI.Modules.Certificates.Reports
{
    public class CertificateReportViewModel : ReactiveScreen
    {
        public CertificateReportViewModel(Certificate certificate)
        {
            Certificate = certificate;

            Instruments = new List<InstrumentViewModel>();
            var row = 1;
            foreach (var instr in Certificate.Instruments.OrderBy(x => x.TestDateTime))
            {
                Instruments.Add(new InstrumentViewModel(instr, row));
                row++;
            }
        }

        public Certificate Certificate { get; set; }

        public string CertificateDate => Certificate.CreatedDateTime.ToShortDateString();

        public List<InstrumentViewModel> Instruments { get; set; }
    }
}