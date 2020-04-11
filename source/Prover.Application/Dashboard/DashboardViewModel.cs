using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using DynamicData.Binding;
using Prover.Application.Interfaces;
using ReactiveUI;

namespace Prover.Application.Dashboard
{
    public class DashboardViewModel : ReactiveObject
    {
        public ICollection<IDashboardValueViewModel> DashboardItems { get; }

        public DashboardViewModel(
                IEnumerable<IDashboardValueViewModel> dashboardItems,
                IEnumerable<ICacheManager> caches)
        {
            
            DashboardItems = dashboardItems.ToList();

            LoadCaches = ReactiveCommand.CreateFromObservable(() =>
            {
                caches.ForEach(c => c.Update()).ToObservable();
                return Observable.Return(Unit.Default);
            });
        }

        public ReactiveCommand<Unit, Unit> LoadCaches { get; }
    }
}
