using System.Collections.Generic;
using System.Linq;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Clients;
using Prover.Core.Settings;
using Prover.GUI.Screens.Modules.Certificates.Common;
using ReactiveUI;

namespace Prover.GUI.Screens.Modules.Certificates.Reports
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

        public Client Client => Instruments.First().Client;
        public Certificate Certificate { get; set; }

        public string McRegistrationNumber =>
            SettingsManager.SharedSettingsInstance.CertificateSettings.McRegistrationNumber;

        public string CertificateDate => $"{Certificate.CreatedDateTime:dd/MM/yyyy}";
        public long NumberOfTestsPassed => Certificate.Instruments.Count(i => i.HasPassed);
        public long NumberOfTestsFailed => Certificate.Instruments.Count(i => !i.HasPassed);
        private List<VerificationViewModel> _instruments;

        public List<VerificationViewModel> Instruments
        {
            get => _instruments;
            set => this.RaiseAndSetIfChanged(ref _instruments, value);
        }
    }
}