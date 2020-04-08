using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Prover.Application.Interfaces;
using Prover.Application.ViewModels;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Client.Desktop.Wpf.Reports
{
    public class ReportViewModel : RoutableViewModelBase
    {
        public ReportViewModel(IScreenManager screen, ReactiveObject contentViewModel = null)
            : base(screen, "Report")
        {
            HostScreen = screen;
            ContentViewModel = contentViewModel;
        }

        [Reactive] public ReactiveObject ContentViewModel { get; set; }
    }
}
