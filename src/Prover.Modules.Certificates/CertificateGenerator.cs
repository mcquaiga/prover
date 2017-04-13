using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using Autofac;
using Prover.Core.ExternalIntegrations;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;
using Prover.Modules.CertificatesUi.Reports;

namespace Prover.Modules.CertificatesUi
{
    public class CertificateGenerator : IExportTestRun
    {
        private readonly Certificate _certificate;
        private readonly IContainer _container;
        private string _filePath;

        public CertificateGenerator(Certificate certificate, IContainer container)
        {
            _certificate = certificate;
            _container = container;
            CreateFileName();
        }

        public string FileName => "certificate_" + _certificate.Number + ".xps";
        public string CertificateFolderPath => Path.Combine(Directory.GetCurrentDirectory(), "Certificates");

        public void Generate()
        {
            //Set up the WPF Control to be printed
            var controlToPrint = new CertificateReportView();
            controlToPrint.DataContext = new CertificateReportViewModel(_certificate);

            var fixedDoc = new FixedDocument();
            fixedDoc.DocumentPaginator.PageSize = new Size(96*11, 96*8.5);
            var pageContent = new PageContent();
            var fixedPage = new FixedPage {Width = 96*11, Height = 96*8.5};

            //Create first page of document
            fixedPage.Children.Add(controlToPrint);
            ((IAddChild) pageContent).AddChild(fixedPage);
            fixedDoc.Pages.Add(pageContent);

            // Save document
            WriteDocument(fixedDoc, _filePath);

            //View the document
            Process.Start(_filePath);
        }

        private void CreateFileName()
        {
            _filePath = Path.Combine(CertificateFolderPath, FileName);

            //Create the directory if it doesn't exist
            if (!Directory.Exists(CertificateFolderPath))
                Directory.CreateDirectory(CertificateFolderPath);
        }

        private string WriteDocument(FixedDocument fixedDoc, string filePath)
        {
            var xpsWriter = new XpsDocument(filePath, FileAccess.ReadWrite);
            var xw = XpsDocument.CreateXpsDocumentWriter(xpsWriter);
            xw.Write(fixedDoc);
            xpsWriter.Close();

            return filePath;
        }

        public Task<bool> Export(Instrument instrumentForExport)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> Export(IEnumerable<Instrument> instrumentsForExport)
        {
            throw new System.NotImplementedException();
        }
    }
}