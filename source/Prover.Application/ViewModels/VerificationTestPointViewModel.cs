using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Devices.Core.Items;
using DynamicData;
using Prover.Application.ViewModels.Corrections;
using Prover.Application.ViewModels.Volume;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels
{
    public class VerificationTestPointViewModel : VerificationViewModel
    {
        private readonly SourceCache<ItemValue, int> _items = new SourceCache<ItemValue, int>(v => v.Id);

        public IObservable<IChangeSet<ItemValue, int>> Connect() => _items.Connect();

        public VerificationTestPointViewModel() { }

        [Reactive] public int TestNumber { get; set; }

        //public extern bool Verified { [ObservableAsProperty] get; }

        public ICollection<VerificationViewModel> VerificationTests { get; set; } = new List<VerificationViewModel>();

        public PressureFactorViewModel Pressure => VerificationTests.OfType<PressureFactorViewModel>().FirstOrDefault();

        public TemperatureFactorViewModel Temperature => VerificationTests.OfType<TemperatureFactorViewModel>().FirstOrDefault();

        public SuperFactorViewModel SuperFactor => VerificationTests.OfType<SuperFactorViewModel>().FirstOrDefault();

        public VolumeViewModelBase Volume => VerificationTests.OfType<VolumeViewModelBase>().FirstOrDefault();

        public void UpdateItemValues(ICollection<ItemValue> itemValues)
        {
            _items.Edit(update => update.AddOrUpdate(itemValues));
        }

        public void AddTest(VerificationViewModel test)
        {
            VerificationTests.Add(test);
            Initialize();
        }

        public void Initialize()
        {
            RegisterVerificationsForVerified(VerificationTests);
            //VerificationTests.AsObservableChangeSet()
            //    .AutoRefresh(model => model.Verified, TimeSpan.FromMilliseconds(25))
            //    .ToCollection()            
            //    .Select(x => x.All(y => y.Verified))
            //    .ToPropertyEx(this, x => x.Verified);
        }

        protected override void Disposing()
        {
            VerificationTests.ForEach(t => t.DisposeWith(Cleanup));
            VerificationTests.Clear();
        }
    }
}