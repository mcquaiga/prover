using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Xps.Packaging;
using Microsoft.Practices.Unity;
using Prover.Core.Models.Instruments;

namespace Prover.GUI.Reports
{
    public class InstrumentGenerator
    {
        private readonly IUnityContainer _container;
        private readonly Instrument _instrument;
        private string _filePath;

        public InstrumentGenerator(Instrument instrument, IUnityContainer container)
        {
            _instrument = instrument;
            _container = container;
            CreateFileName();
        }

        public string FileName
            =>
                string.Format("{0}-instrument-{1}.xps", _instrument.SerialNumber,
                    DateTime.Now.ToFileTime().ToString().Substring(0, 10));

        public string InstrumentsFolderPath => Path.Combine(Directory.GetCurrentDirectory(), "InstrumentReports");

        public void Generate()
        {
            //Set up the WPF Control to be printed
            var controlToPrint = new InstrumentReportView();
            controlToPrint.DataContext = new InstrumentReportViewModel(_container, _instrument);

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
            _filePath = Path.Combine(InstrumentsFolderPath, FileName);

            //Create the directory if it doesn't exist
            if (!Directory.Exists(InstrumentsFolderPath))
            {
                Directory.CreateDirectory(InstrumentsFolderPath);
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