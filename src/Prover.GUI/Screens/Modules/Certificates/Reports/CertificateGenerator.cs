using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Xps.Packaging;
using Caliburn.Micro;
using Prover.Core.Models.Certificates;

namespace Prover.GUI.Screens.Modules.Certificates.Reports
{
    public class CertificateGenerator
    {
        private static string FileName(long certNumber)
        {
            return $"certificate_{certNumber}.xps";
        }

        public static string CertificateFolderPath => Path.Combine(Directory.GetCurrentDirectory(), "Certificates");

        public static void GenerateXps(Certificate certificate)
        {
            var filePath = CreateFileName(certificate);

            var fixedDoc = CreateFixedDocument(certificate);

                // Save document
            WriteDocumentToFile(fixedDoc, filePath);

            //View the document
            Process.Start(filePath);

        }

        private static FixedDocument CreateFixedDocument(Certificate certificate)
        {
            //Set up the WPF Control to be printed
            var viewModel = IoC.Get<CertificateReportViewModel>();
            viewModel.Initialize(certificate);
            var controlToPrint = new CertificateReportView
            {
                DataContext = viewModel
            };

            var fixedDoc = new FixedDocument();
            fixedDoc.DocumentPaginator.PageSize = new Size(96 * 11, 96 * 8.5);
            var pageContent = new PageContent();
            var fixedPage = new FixedPage {Width = 96 * 11, Height = 96 * 8.5};

            //Create first page of document
            fixedPage.Children.Add(controlToPrint);
            ((IAddChild) pageContent).AddChild(fixedPage);
            fixedDoc.Pages.Add(pageContent);
            return fixedDoc;
        }

        private static string CreateFileName(Certificate certificate)
        {
            var filePath = Path.Combine(CertificateFolderPath, FileName(certificate.Number));

            //Create the directory if it doesn't exist
            if (!Directory.Exists(CertificateFolderPath))
                Directory.CreateDirectory(CertificateFolderPath);

            if (File.Exists(filePath))
                File.Delete(filePath);

            return filePath;
        }

        private static MemoryStream WriteDocumentToMemoryStream(FixedDocument fixedDoc)
        {
            var memoryStream = new MemoryStream();
            using (var package = Package.Open(memoryStream, FileMode.Create))
            {
                using (var doc = new XpsDocument(package))
                {
                    var writer = XpsDocument.CreateXpsDocumentWriter(doc);
                    writer.Write(fixedDoc);
                    doc.Close();
                }
            }

            return memoryStream;
        }

        private static void WriteDocumentToFile(FixedDocument fixedDoc, string filePath)
        {
            using (var xpsWriter = new XpsDocument(filePath, FileAccess.ReadWrite))
            {
                var xw = XpsDocument.CreateXpsDocumentWriter(xpsWriter);
                xw.Write(fixedDoc);
                xpsWriter.Close();
                fixedDoc = null;
            }
        }
    }
}