using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Application.Services;
using Client.Wpf.ViewModels;
using Domain.EvcVerifications;
using ReactiveUI;
using Shared.Interfaces;

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
