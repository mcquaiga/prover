using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Devices.Core.Items;
using DynamicData;
using Prover.Application.Extensions;
using Prover.Application.ViewModels.Corrections;
using Prover.Application.ViewModels.Volume;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels
{
    public class VerificationTestPointViewModel : ViewModelWithIdBase
    {

        private readonly SourceCache<ItemValue, int> _items = new SourceCache<ItemValue, int>(v => v.Id);

        public IObservable<IChangeSet<ItemValue, int>> Connect() => _items.Connect();
        public VerificationTestPointViewModel()
        {
        }

        [Reactive] public int TestNumber { get; set; }

        public ICollection<VerificationViewModel> TestsCollection { get; set; } = new List<VerificationViewModel>();

        public PressureFactorViewModel Pressure => TestsCollection.OfType<PressureFactorViewModel>().FirstOrDefault();

        public TemperatureFactorViewModel Temperature => TestsCollection.OfType<TemperatureFactorViewModel>().FirstOrDefault();

        public SuperFactorViewModel SuperFactor => TestsCollection.OfType<SuperFactorViewModel>().FirstOrDefault();

        public VolumeViewModelBase Volume => TestsCollection.OfType<VolumeViewModelBase>().FirstOrDefault();

        public void UpdateItemValues(ICollection<ItemValue> itemValues)
        {
            _items.Edit(update => update.AddOrUpdate(itemValues));
        }

        public void AddTest(VerificationViewModel test)
        {

        }

        protected override void Disposing()
        {
            TestsCollection.ForEach(t => t.DisposeWith(Cleanup));
            TestsCollection.Clear();
        }
    }
}