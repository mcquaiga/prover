using System;
using System.Collections.Generic;
using System.Text;
using Prover.Application.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.Reports
{
    public class ReportViewModel : ReactiveObject, IRoutableViewModel
    {
        public ReportViewModel(IScreenManager screen)
        {
            HostScreen = screen;
         
        }

        [Reactive] public ReactiveObject ContentViewModel { get; set; }
        public string UrlPathSegment { get; } = "Report";
        public IScreen HostScreen { get; }
    }
}
