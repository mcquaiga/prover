using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Xps.Packaging;
using Prover.Core.Models.Instruments;
using Prover.GUI.Common;

namespace Prover.GUI.Reports
{
    public class InstrumentGenerator
    {
        private readonly ScreenManager _screenManager;

        public InstrumentGenerator(ScreenManager screenManager)
        {
            _screenManager = screenManager;   
        }

        public string InstrumentsFolderPath => Path.Combine(Directory.GetCurrentDirectory(), "InstrumentReports");

        public async Task Generate(Instrument instrument)
        {
            var filePath = CreateFileName(instrument);

            //Set up the WPF Control to be printed
            var controlToPrint = new InstrumentReportView();
            var reportViewModel = _screenManager.ResolveViewModel<InstrumentReportViewModel>();
            controlToPrint.DataContext = reportViewModel;
            
            await reportViewModel.Initialize(instrument);

            var fixedDoc = new FixedDocument();
            fixedDoc.DocumentPaginator.PageSize = new Size(96*11, 96*8.5);
            var pageContent = new PageContent();
            var fixedPage = new FixedPage {Width = 96*11, Height = 96*8.5};

            //Create first page of document
            fixedPage.Children.Add(controlToPrint);
            ((IAddChild) pageContent).AddChild(fixedPage);
            fixedDoc.Pages.Add(pageContent);

            // Save document
            WriteDocument(fixedDoc, filePath);

            //View the document
            Process.Start(filePath);
        }

        private string CreateFileName(Instrument instrument)
        {
            var fileName = string.Format("{0}-instrument-{1}.xps", instrument.SerialNumber,
                    DateTime.Now.ToFileTime().ToString().Substring(0, 10));

            var filePath = Path.Combine(InstrumentsFolderPath, fileName);

            //Create the directory if it doesn't exist
            if (!Directory.Exists(InstrumentsFolderPath))
            {
                Directory.CreateDirectory(InstrumentsFolderPath);
            }

            return filePath;
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