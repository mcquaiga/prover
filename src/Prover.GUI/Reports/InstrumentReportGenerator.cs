using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Xps.Packaging;
using Prover.Core.Models.Instruments;
using Prover.Core.Storage;
using Prover.GUI.Screens;

namespace Prover.GUI.Reports
{
    public class InstrumentReportGenerator
    {
        private readonly ScreenManager _screenManager;

        public InstrumentReportGenerator(ScreenManager screenManager)
        {
            _screenManager = screenManager;
        }

        public string OutputFolderPath => Path.Combine(Directory.GetCurrentDirectory(), "InstrumentReports");

        public async Task GenerateAndViewReport(Instrument instrument)
        {
            var filePath = CreateFileName(instrument);

            //Set up the WPF Control to be printed            
            var reportViewModel = _screenManager.ResolveViewModel<InstrumentReportViewModel>();
            await reportViewModel.Initialize(instrument);

            var controlToPrint = new InstrumentReportView
            {
                DataContext = reportViewModel
            };

            //Create first page of document
            var fixedPage = new FixedPage {Width = 96 * 11, Height = 96 * 8.5};                 
            fixedPage.Children.Add(controlToPrint);

            var pageContent = new PageContent();
            ((IAddChild) pageContent).AddChild(fixedPage);

            var fixedDoc = new FixedDocument();
            fixedDoc.DocumentPaginator.PageSize = new Size(96 * 11, 96 * 8.5);
            fixedDoc.Pages.Add(pageContent);

            // Save document
            WriteDocument(fixedDoc, filePath);

            //View the document
            Process.Start(filePath);
        }

        private string CreateFileName(Instrument instrument)
        {
            var fileName =
                $"instrument_{instrument.InventoryNumber}_{DateTime.UtcNow.ToFileTimeUtc().ToString().Substring(0, 8)}.xps";

            var filePath = Path.Combine(OutputFolderPath, fileName);

            //Create the directory if it doesn't exist
            if (!Directory.Exists(OutputFolderPath))
                Directory.CreateDirectory(OutputFolderPath);

            return filePath;
        }

        private void WriteDocument(FixedDocument fixedDoc, string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var xpsWriter = new XpsDocument(filePath, FileAccess.ReadWrite);
            var xw = XpsDocument.CreateXpsDocumentWriter(xpsWriter);
            xw.Write(fixedDoc);
            xpsWriter.Close();
        }
    }
}