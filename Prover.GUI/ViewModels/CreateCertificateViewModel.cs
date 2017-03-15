using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using Caliburn.Micro.ReactiveUI;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.GUI.ViewModels.CertificateReport;
using Prover.GUI.ViewModels.InstrumentsList;
using Prover.GUI.Views.CertificateReport;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Prover.GUI.ViewModels
{
    public class CreateCertificateViewModel: ReactiveScreen
    {
        private IUnityContainer _container;

        public List<string> VerificationTypes
        {
            get
            {
                return new List<string> {"Verification", "Re-Verification"};
            }
        } 

        public CreateCertificateViewModel(IUnityContainer container)
        {
            _container = container;
            InstrumentsListViewModel = new InstrumentsListViewModel(_container);
        }

        public InstrumentsListViewModel InstrumentsListViewModel { get; private set; }

        //public Certificate Certificate { get; set; }

        public string CreatedDateTime { get; set; }
        public string VerificationType { get; set; }
        public string TestedBy { get; set; }

        public string SealExpirationDate
        {
            get { return DateTime.Now.AddYears(5).ToString("yyyy-MM-dd"); }
        }

        public void OneWeekFilter()
        {
            InstrumentsListViewModel.GetInstrumentsWithNoCertificateLastWeek();
        }

        public void OneMonthFilter()
        {
            InstrumentsListViewModel.GetInstrumentsWithNoCertificateLastMonth();
        }

        public void ResetFilter()
        {
            InstrumentsListViewModel.GetInstrumentsByCertificateId(null);
        }

        public int InstrumentCount 
        {
            get { return InstrumentsListViewModel.InstrumentItems.Count(x => x.IsSelected);  }
        }

        public void CreateCertificate()
        {
            var selectedInstruments = InstrumentsListViewModel.InstrumentItems.Where(x => x.IsSelected == true).ToList();

            if (selectedInstruments.Count() > 8)
            {
                MessageBox.Show("Maximum 8 instruments allowed per certificate.");
                return;
            }

            if (!selectedInstruments.Any())
            {
                MessageBox.Show("Please select at least one instrument.");
                return;
            }

            if (VerificationType == null || TestedBy == null)
            {
                MessageBox.Show("Please enter a tested by and verificate type.");
                return;
            }

            var instruments = InstrumentsListViewModel.InstrumentItems.Where(x => x.IsSelected).Select(i => i.Instrument).ToList();
            var certificate = Certificate.CreateCertificate(_container, TestedBy, VerificationType, instruments);
            
            InstrumentsListViewModel.GetInstrumentsByCertificateId(null);

            GenerateReport(certificate);
        }

        public long? ExistingCertificateNumber { get; set; }
        public void PrintExistingCertificate()
        {
            LoadCertificate(ExistingCertificateNumber);
        }

        private void LoadCertificate(long? certificateId)
        {
            if (!certificateId.HasValue) return;

            var cert = Certificate.FindCertificate(_container, certificateId.Value);

            if (cert != null)
            {
                GenerateReport(cert);
            }

            ExistingCertificateNumber = null;
            NotifyOfPropertyChange(() => ExistingCertificateNumber);
        }

        private void GenerateReport(Certificate certificate)
        {
            //Set up the WPF Control to be printed
            var controlToPrint = new CertificateReportView
            {
                DataContext = new CertificateReportViewModel(_container, certificate)
            };

            FixedDocument fixedDoc = new FixedDocument();
            fixedDoc.DocumentPaginator.PageSize = new Size(96 * 11, 96 * 8.5);
            PageContent pageContent = new PageContent();
            FixedPage fixedPage = new FixedPage();
            fixedPage.Width = 96 * 11;
            fixedPage.Height = 96 * 8.5;

            //Create first page of document
            fixedPage.Children.Add(controlToPrint);
            ((System.Windows.Markup.IAddChild) pageContent).AddChild(fixedPage);
            fixedDoc.Pages.Add(pageContent);
            //Create any other required pages here

            //var dlg = new Microsoft.Win32.SaveFileDialog { FileName = "Certificate", DefaultExt = ".xps", Filter = "XPS Documents (.xps)|*.xps" };
            //bool? result = dlg.ShowDialog();

            //// Process save file dialog box results
            //if (result == true)
            //{
            // Save document
            var path = Directory.GetCurrentDirectory();
            string filename = "certificate_" + certificate.Number + ".xps";
            var filePath = Path.Combine(path, "Certificates");
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
            filePath = Path.Combine(filePath, filename);

            if (File.Exists(filePath)) File.Delete(filePath);

            //View the document
            var xpsWriter = new XpsDocument(filePath, FileAccess.ReadWrite);
            var xw = XpsDocument.CreateXpsDocumentWriter(xpsWriter);
            xw.Write(fixedDoc);
            xpsWriter.Close();

            System.Diagnostics.Process.Start(filePath);
            //}
        }
    }
}
