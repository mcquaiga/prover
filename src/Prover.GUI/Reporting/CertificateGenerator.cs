using Prover.GUI.ViewModels.CertificateReport;
using Prover.GUI.Views.CertificateReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Certificates;
using Prover.Core.Models.Instruments;
using System.Windows.Documents;
using System.Windows;
using System.IO;
using System.Windows.Xps.Packaging;
using Caliburn.Micro.ReactiveUI;
using Caliburn.Micro;

namespace Prover.GUI.Reporting
{
    public class CertificateGenerator
    {
        private readonly IUnityContainer _container;
        private readonly Certificate _certificate;
        private string _filePath;
        public CertificateGenerator(Certificate certificate, IUnityContainer container)
        {
            _certificate = certificate;
            _container = container;
            CreateFileName();
        }

        public void Generate()
        {          

            //Set up the WPF Control to be printed
            var controlToPrint = new CertificateReportView();
            controlToPrint.DataContext = new CertificateReportViewModel(_container, _certificate);

            var fixedDoc = new FixedDocument();
            fixedDoc.DocumentPaginator.PageSize = new Size(96 * 11, 96 * 8.5);
            var pageContent = new PageContent();
            var fixedPage = new FixedPage { Width = 96 * 11, Height = 96 * 8.5 };

            //Create first page of document
            fixedPage.Children.Add(controlToPrint);
            ((System.Windows.Markup.IAddChild)pageContent).AddChild(fixedPage);
            fixedDoc.Pages.Add(pageContent);

            // Save document
            WriteDocument(fixedDoc, _filePath);
            
            //View the document
            System.Diagnostics.Process.Start(_filePath);
        }

        public string FileName => "certificate_" + _certificate.Number + ".xps";
        public string CertificateFolderPath => Path.Combine(Directory.GetCurrentDirectory(), "Certificates");

        void CreateFileName()
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
