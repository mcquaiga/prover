using System.Collections.Generic;
using System.Linq;
using Prover.Core.Models.Certificates;
using Prover.GUI.Common.Screens;
using Prover.GUI.Modules.Certificates.Common;
using ReactiveUI;

namespace Prover.GUI.Modules.Certificates.Reports
{
    public class CertificateReportViewModel : ViewModelBase
    {
        public CertificateReportViewModel(Certificate certificate)
        {
            Certificate = certificate;

            Instruments = new List<VerificationViewModel>();
            var row = 1;
            foreach (var instr in Certificate.Instruments.OrderBy(x => x.TestDateTime))
            {
                Instruments.Add(new VerificationViewModel(instr, row));
                row++;
            }
        }

        public Certificate Certificate { get; set; }

        public string CertificateDate => Certificate.CreatedDateTime.ToShortDateString();

        private List<VerificationViewModel> _instruments;

        public List<VerificationViewModel> Instruments
        {
            get { return _instruments; }
            set { this.RaiseAndSetIfChanged(ref _instruments, value); }
        }
    }
}