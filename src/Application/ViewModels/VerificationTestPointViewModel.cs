using System;
using System.Collections.Generic;
using System.Linq;
using Application.ViewModels.Corrections;
using Application.ViewModels.Volume;
using Devices.Core.Items;
using DynamicData;
using ReactiveUI.Fody.Helpers;

namespace Application.ViewModels
{
    public class VerificationTestPointViewModel : BaseViewModel
    {

        private readonly SourceCache<ItemValue, int> _items = new SourceCache<ItemValue, int>(v => v.Id);
        private ICollection<VerificationViewModel> _testsCollection = new List<VerificationViewModel>();

        public IObservable<IChangeSet<ItemValue, int>> Connect() => _items.Connect();
        public VerificationTestPointViewModel()
        {
        }

        [Reactive] public int TestNumber { get; set; }

        public ICollection<VerificationViewModel> TestsCollection
        {
            get => _testsCollection;
            set => _testsCollection = value;
        }

        public PressureFactorViewModel Pressure => TestsCollection.OfType<PressureFactorViewModel>().FirstOrDefault();

        public TemperatureFactorViewModel Temperature => TestsCollection.OfType<TemperatureFactorViewModel>().FirstOrDefault();

        public SuperFactorViewModel SuperFactor => TestsCollection.OfType<SuperFactorViewModel>().FirstOrDefault();

        public VolumeViewModel Volume => TestsCollection.OfType<VolumeViewModel>().FirstOrDefault();

        public void UpdateItemValues(ICollection<ItemValue> itemValues)
        {
            _items.Edit(update => update.AddOrUpdate(itemValues));
        }

        public void AddTest(VerificationViewModel test)
        {

        }
    }
}