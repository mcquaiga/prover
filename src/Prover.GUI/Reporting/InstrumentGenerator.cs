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
using Prover.GUI.Views.InstrumentReport;
using Prover.GUI.ViewModels.InstrumentReport;

namespace Prover.GUI.Reporting
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

        public void Generate()
        {
            //Set up the WPF Control to be printed
            var controlToPrint = new InstrumentReportView();
            controlToPrint.DataContext = new InstrumentReportViewModel(_container, _instrument);

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

        public string FileName => string.Format("instrument-{0}-{1}.xps", _instrument.SerialNumber, DateTime.Now.ToFileTime());
        public string InstrumentsFolderPath => Path.Combine(Directory.GetCurrentDirectory(), "InstrumentReports");

        void CreateFileName()
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
