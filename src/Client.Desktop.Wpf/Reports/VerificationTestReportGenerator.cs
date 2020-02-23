using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Application.Settings;
using Application.ViewModels;
using Client.Desktop.Wpf.ViewModels;
using Client.Wpf.ViewModels.Verifications;
using Client.Wpf.Views.Verifications;

namespace Client.Desktop.Wpf.Reports
{
    public class VerificationTestReportGenerator
    {
        private readonly IScreenManager _screenManager;
        private readonly ISettingsService _settingsService;

        public VerificationTestReportGenerator(IScreenManager screenManager, ISettingsService settingsService)
        {
            _screenManager = screenManager;
            _settingsService = settingsService;
        }

        public async Task GenerateAndViewReport(EvcVerificationViewModel verificationTest)
        {
            var fixedDoc = new FixedDocument();
            fixedDoc.DocumentPaginator.PageSize = new Size(96 * 11, 96 * 8.5);

            var reportViewModel = new TestDetailsViewModel(_screenManager, verificationTest);
            var reportView = new TestDetailsView {ViewModel = reportViewModel};
        }
    }
}
