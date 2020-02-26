using Client.Desktop.Wpf.ViewModels;
using Prover.Application.Services;
using ReactiveUI;

namespace Modules.UnionGas.Exporter.Views
{
    public class ExporterViewModel : ReactiveObject, IRoutableViewModel
    {
        private readonly VerificationViewModelService _service;
        public IScreenManager ScreenManager { get; }

        public ExporterViewModel(IScreenManager screenManager, VerificationViewModelService service)
        {
            _service = service;
            ScreenManager = screenManager;
            HostScreen = screenManager;
        }
        public string UrlPathSegment => "/Exporter";
        public IScreen HostScreen { get; }
    }
}
