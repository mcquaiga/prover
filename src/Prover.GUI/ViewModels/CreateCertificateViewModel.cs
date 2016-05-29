using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Certificates;
using Prover.GUI.Reporting;
using Prover.GUI.ViewModels.InstrumentsList;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;

namespace Prover.GUI.ViewModels
{
    public class CreateCertificateViewModel: ReactiveScreen
    {
        private IUnityContainer _container;

        public List<string> VerificationTypes => new List<string> {"Verification", "Re-Verification"};

        public CreateCertificateViewModel(IUnityContainer container)
        {
            _container = container;
            GetInstrumentsByCertificateId(null);
        }

        public Certificate Certificate { get; set; }

        public DateTime CreatedDateTime { get; set; }
        public string VerificationType { get; set; }
        public string TestedBy { get; set; }

        //public string SealExpirationDate
        //{
        //    get
        //    {
        //        var period = 10; //Re-Verification
        //        if (VerificationType == "Verification")
        //        {
        //            period = 12;
        //        }

        //        return DateTime.Now.AddYears(period).ToString("yyyy-MM-dd");
        //    }
        //}

        public void OneWeekFilter()
        {
            GetInstrumentsWithNoCertificateLastWeek();
        }

        public void OneMonthFilter()
        {
            GetInstrumentsWithNoCertificateLastMonth();
        }

        public void ResetFilter()
        {
            GetInstrumentsByCertificateId(null);
        }

        public int InstrumentCount 
        {
            get { return InstrumentItems.Count(x => x.IsSelected);  }
        }

        public async Task ExportQARuns()
        {
            var instruments = InstrumentItems.Where(x => x.IsSelected).Select(i => i.Instrument).ToList();

            foreach(var i in instruments)
            {

            }
        }

        public ObservableCollection<InstrumentTestGridViewModel> InstrumentItems { get; set; } = new ObservableCollection<InstrumentTestGridViewModel>();

        public void GetInstrumentsByCertificateId(Guid? certificateGuid)
        {
            GetInstrumentVerificationTests(x => x.CertificateId == certificateGuid);
        }

        public void GetInstrumentsWithNoCertificateLastMonth()
        {
            var dateFilter = DateTime.Now.AddDays(-30);
            GetInstrumentVerificationTests(x => x.CertificateId == null && x.TestDateTime >= dateFilter);
        }

        public void GetInstrumentsWithNoCertificateLastWeek()
        {
            var dateFilter = DateTime.Now.AddDays(-7);
            GetInstrumentVerificationTests(x => x.CertificateId == null && x.TestDateTime >= dateFilter);
        }

        private void GetInstrumentVerificationTests(Func<Instrument, bool> whereFunc)
        {
            InstrumentItems.Clear();

            using (var store = _container.Resolve<IInstrumentStore<Instrument>>())
            {
                var instruments = store.Query()
                                    .Where(whereFunc)
                                    .OrderBy(i => i.TestDateTime)
                                    .ToList();

                instruments.ForEach(i => InstrumentItems.Add(new InstrumentTestGridViewModel(_container, i)));
            }

            NotifyOfPropertyChange(() => InstrumentItems);
        }

        //public void CreateCertificate()
        //{
        //    var instruments = InstrumentsListViewModel.InstrumentItems.Where(x => x.IsSelected).Select(i => i.Instrument).ToList();

        //    if (instruments.Count() > 8)
        //    {
        //        MessageBox.Show("Maximum 8 instruments allowed per certificate.");
        //        return;
        //    }

        //    if (!instruments.Any())
        //    {
        //        MessageBox.Show("Please select at least one instrument.");
        //        return;
        //    }

        //    if (VerificationType == null || TestedBy == null)
        //    {
        //        MessageBox.Show("Please enter a tested by and verificate type.");
        //        return;
        //    }



        //    var cert = Certificate.CreateCertificate(_container, TestedBy, VerificationType, instruments);

        //    var generator = new CertificateGenerator(cert, _container);
        //    generator.Generate();

        //    InstrumentsListViewModel.GetInstrumentsByCertificateId(null);
        //}
    }
}
