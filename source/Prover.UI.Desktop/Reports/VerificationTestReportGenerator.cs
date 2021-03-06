﻿using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Xps.Packaging;
using Devices.Core.Items.ItemGroups;
using Prover.Application;
using Prover.Application.Interfaces;
using Prover.Application.Mappers;
using Prover.Application.Models.EvcVerifications;
using Prover.Application.Settings;
using Prover.Application.ViewModels;
using ReactiveUI;

namespace Prover.UI.Desktop.Reports
{
	public class VerificationTestReportGenerator
	{
		private readonly IScreenManager _screenManager;
		private readonly ISettingsService _settingsService;
		private string OutputFolderPath => Path.Combine(AppDefaults.AppDataDirectory, "Reports");

		public VerificationTestReportGenerator(IScreenManager screenManager, ISettingsService settingsService)
		{
			_screenManager = screenManager;
			_settingsService = settingsService;
		}

		public Task GenerateAndViewReport(EvcVerificationViewModel verificationTest)
		{
			var locator = ViewLocator.Current;

			var reportViewModel = new ReportViewModel(_screenManager)
			{
				ContentViewModel = verificationTest
			};

			return _screenManager.ChangeView(reportViewModel);

			//var filePath = CreateFileName(verificationTest);

			//var fixedDoc = new FixedDocument();
			//fixedDoc.DocumentPaginator.PageSize = new Size(96 * 11, 96 * 8.5);
			//fixedDoc.Pages.Add(CreatePage(reportView));

			//WriteDocument(fixedDoc, filePath);
		}

		public Task GenerateAndViewReport(EvcVerificationTest verificationTest)
		{
			return GenerateAndViewReport(verificationTest.ToViewModel());
		}

		//public async Task GeneratePdfReport(EvcVerificationViewModel test)
		//{
		//    var filePath = CreateFileName(test);
		//    //var doc = await _reportingService.ExportAsync<TestDetailsView>(DocumentFormat.Pdf, filePath);
		//}

		//private void CreatePdf(IViewFor reportView, string filePath)
		//{

		//    var fixedDoc = new FixedDocument();
		//    fixedDoc.DocumentPaginator.PageSize = new Size(96 * 11, 96 * 8.5);
		//    fixedDoc.Pages.Add(CreatePage((UIElement)reportView));

		//    //GlobalFontSettings.FontResolver = new FontResolver();
		//    var lMemoryStream = new MemoryStream();
		//    var package = Package.Open(lMemoryStream, FileMode.Create);
		//    var doc = new XpsDocument(package);
		//    var writer = XpsDocument.CreateXpsDocumentWriter(doc);

		//    writer.Write(fixedDoc);

		//    doc.Close();
		//    package.Close();

		//    //PdfSharp.Pdf.Xps.XpsModel.XpsDocument.Open(lMemoryStream);
		//}

		private PageContent CreatePage(object testControl)
		{
			var pageContent = new PageContent();
			var fixedPage = new FixedPage { Width = 96 * 11, Height = 96 * 8.5 };
			//Create first page of document
			fixedPage.Children.Add((UserControl)testControl);
			((IAddChild)pageContent).AddChild(fixedPage);
			return pageContent;
		}

		private string CreateFileName(EvcVerificationViewModel test)
		{
			var serial = test.Device.ItemGroup<SiteInformationItems>();

			var fileName =
				$"Test-{serial.SerialNumber}-{DateTime.Now.ToFileTime().ToString().Substring(0, 10)}.xps";

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



