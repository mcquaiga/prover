using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Xps.Packaging;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Certificates;

namespace Prover.GUI.Reports
{
    public class CertificateGenerator
    {
        private readonly Certificate _certificate;
        private readonly IUnityContainer _container;
        private string _filePath;

        public CertificateGenerator(Certificate certificate, IUnityContainer container)
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
            controlToPrint.DataContext = new CertificateReportViewModel(_container, _certificate);

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
            {
                Directory.CreateDirectory(CertificateFolderPath);
            }
        }

        private string WriteDocument(FixedDocument fixedDoc, string filePath)
        {
            var xpsWriter = new XpsDocument(filePath, FileAccess.ReadWrite);
            var xw = XpsDocument.CreateXpsDocumentWriter(xpsWriter);
            xw.Write(fixedDoc);
            xpsWriter.Close();

            return filePath;
        }
    }
}