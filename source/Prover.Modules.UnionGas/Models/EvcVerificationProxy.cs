using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Devices.Core.Interfaces;
using Prover.Domain.EvcVerifications;
using Prover.Modules.UnionGas.DcrWebService;
using Prover.Modules.UnionGas.Exporter.Views;
using Prover.Shared.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Prover.Modules.UnionGas.Models
{
    public class EvcVerificationProxy : ReactiveObject
    {
        private readonly IObservable<EvcVerificationTest> _changeUpdates;

        public EvcVerificationProxy(EvcVerificationTest verification, IObservable<EvcVerificationTest> changeUpdates, ILoginService<EmployeeDTO> loginService)
        {
            _changeUpdates = changeUpdates;

            this.WhenAnyObservable(x => x._changeUpdates)
                .Where(x => x.Id == Test.Id)
                .ToPropertyEx(this, x => x.Test, verification);

            var initEmployee = loginService?.GetUsers().FirstOrDefault(u => u.Id == Test.EmployeeId);
            this.WhenAnyValue(x => x.Test)
                .Where(t => !string.IsNullOrEmpty(t.EmployeeId))
                .Select(t => loginService?.GetUsers().FirstOrDefault(u => u.Id == t.EmployeeId))
                .ToPropertyEx(this, x => x.Employee, initEmployee);

            //if (toolbar != null)
            //{
            //    Toolbar = toolbar;
            //    this.WhenAnyValue(x => x.Toolbar.VerificationTest)
            //        .ToPropertyEx(this, x => x.Test, verification);
            //}
        }

        public extern EvcVerificationTest Test { [ObservableAsProperty] get; }

        //[Reactive] public ExportToolbarViewModel Toolbar { get; protected set; }
        
        [Reactive] public bool IsSelected { get; set; }
        
        public string Composition => Test.Device.CompositionShort();

        public extern EmployeeDTO Employee { [ObservableAsProperty]get; }

        public long SerialNumber => Test.Device.SiteInfo().SerialNumber.ToInt32();

        public long InventoryNumber => Test.Device.CompanyNumber().ToInt32();
    }
}