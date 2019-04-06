using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Xps.Packaging;
using Prover.Core.Models.Instruments;
using Prover.Core.Shared.Data;
using Prover.Core.Storage;
using Prover.GUI;
using Prover.GUI.Screens;

namespace Prover.GUI.Reports
{
    public class InstrumentReportGenerator
    {
        private readonly ScreenManager _screenManager;
        private readonly IProverStore<Instrument> _instrumentStore;

        public InstrumentReportGenerator(ScreenManager screenManager, IProverStore<Instrument> instrumentStore)
        {
            _screenManager = screenManager;
            _instrumentStore = instrumentStore;
        }

        public string OutputFolderPath => Path.Combine(Directory.GetCurrentDirectory(), "InstrumentReports");

        public async Task GenerateAndViewReport(Instrument instrument)
        {
            instrument = await _instrumentStore.Get(instrument.Id);
            var linkedInstrument = _instrumentStore.Query(i => i.LinkedTestId == instrument.Id).FirstOrDefault();

            var filePath = CreateFileName(instrument);

            var fixedDoc = new FixedDocument();
            fixedDoc.DocumentPaginator.PageSize = new Size(96 * 11, 96 * 8.5);
                    
            var instrumentControl = await CreateReportControl(instrument);                                 
            fixedDoc.Pages.Add(CreatePage(instrumentControl));

            if (linkedInstrument != null)
            {                
                instrumentControl = await CreateReportControl(linkedInstrument);                                 
                fixedDoc.Pages.Add(CreatePage(instrumentControl));
            }

            // Save document
            WriteDocument(fixedDoc, filePath);

            //View the document
            Process.Start(filePath);
        }

        private PageContent CreatePage(InstrumentReportView instrumentControl)
        {
            var pageContent = new PageContent();
            var fixedPage = new FixedPage { Width = 96 * 11, Height = 96 * 8.5 };
             //Create first page of document
            fixedPage.Children.Add(instrumentControl);
            ((IAddChild)pageContent).AddChild(fixedPage);
            return pageContent;
        }

        private async Task<InstrumentReportView> CreateReportControl(Instrument instrument)
        {
            //Set up the WPF Control to be printed
            var controlToPrint = new InstrumentReportView();
            var reportViewModel = _screenManager.ResolveViewModel<InstrumentReportViewModel>();
            await reportViewModel.Initialize(instrument);
            controlToPrint.DataContext = reportViewModel;
            return controlToPrint;
        }

        private string CreateFileName(Instrument instrument)
        {
            var fileName =
                $"{instrument.InventoryNumber}-instrument-{DateTime.Now.ToFileTime().ToString().Substring(0, 10)}.xps";

            var filePath = Path.Combine(OutputFolderPath, fileName);

            //Create the directory if it doesn't exist
            if (!Directory.Exists(OutputFolderPath))
                Directory.CreateDirectory(OutputFolderPath);

            return filePath;
        }

        private string WriteDocument(FixedDocument fixedDoc, string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var xpsWriter = new XpsDocument(filePath, FileAccess.ReadWrite);
            var xw = XpsDocument.CreateXpsDocumentWriter(xpsWriter);
            xw.Write(fixedDoc);
            xpsWriter.Close();

            return filePath;
        }
    }
}