using System;
using System.Reactive.Linq;
using Devices.Core.Interfaces;
using Prover.Application.Models.EvcVerifications;
using Prover.Modules.UnionGas.DcrWebService;
using Prover.Modules.UnionGas.Exporter.Views;
using Prover.Shared.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Modules.UnionGas.Models
{
    public class VerificationProxy
    {
        public VerificationProxy(EvcVerificationTest verification,
            IObservable<EvcVerificationTest> changeUpdates,
            ILoginService<Employee> loginService,
            IReactiveCommand viewReport)
        {
        }
    }


    public class EvcVerificationProxy : ReactiveObject
    {
        public IReactiveCommand ViewReport { get; }
        private readonly IObservable<EvcVerificationTest> _changeUpdates;

        public EvcVerificationProxy
        (EvcVerificationTest verification, IObservable<EvcVerificationTest> changeUpdates, ILoginService<Employee> loginService, IReactiveCommand viewReport,
                ExportToolbarViewModel toolbarViewModel
        )
        {
            ViewReport = viewReport;
            _changeUpdates = changeUpdates;

            this.WhenAnyObservable(x => x._changeUpdates)
                .Where(x => x.Id == Test.Id)
                .ToPropertyEx(this, x => x.Test, verification);

            //var initEmployee = loginService?.GetUsers().FirstOrDefault(u => u.Id == Test.EmployeeId);
            
            //this.WhenAnyValue(x => x.Test)
            //    .Where(t => !string.IsNullOrEmpty(t.EmployeeId))
            //    .Select(t => loginService?.GetUsers().FirstOrDefault(u => u.Id == t.EmployeeId))
            //    .ToPropertyEx(this, x => x.Employee, initEmployee);

            this.WhenAnyValue(x => x.Test)
                .Select(t => t.ArchivedDateTime.HasValue || t.ExportedDateTime.HasValue)
                .ToPropertyEx(this, x => x.IsLocked);
        }

        public bool Verified => Test.Verified;
        public extern EvcVerificationTest Test { [ObservableAsProperty] get; }
        
        public extern bool IsLocked { [ObservableAsProperty] get; }

        [Reactive] public bool IsSelected { get; set; }
        
        public string Composition => Test.Device.CompositionShort();

        public extern EmployeeDTO Employee { [ObservableAsProperty]get; }

        public long SerialNumber => Test.Device.SiteInfo().SerialNumber.ToInt32();

        public long InventoryNumber => Test.Device.CompanyNumber().ToInt32();
    }
}