using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using Application.ViewModels.Corrections;
using Application.ViewModels.Volume;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using Domain.EvcVerifications.Verifications;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Application.ViewModels
{
    public class VerificationTestPointViewModel : BaseViewModel
    {

        private readonly SourceCache<ItemValue, int> _items = new SourceCache<ItemValue, int>(v => v.Id);
        private ICollection<VerificationViewModel> _testsCollection = new List<VerificationViewModel>();

        // We expose the Connect() since we are interested in a stream of changes.
        // If we have more than one subscriber, and the subscribers are known, 
        // it is recommended you look into the Reactive Extension method Publish().
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