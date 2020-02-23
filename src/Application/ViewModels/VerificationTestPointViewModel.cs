using System;
using System.Collections.Generic;
using System.Linq;
using Application.Extensions;
using Application.ViewModels.Corrections;
using Application.ViewModels.Volume;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Devices.Core.Items.ItemGroups;
using DynamicData;
using ReactiveUI.Fody.Helpers;

namespace Application.ViewModels
{
    public class VerificationTestPointViewModel : BaseViewModel
    {

        private readonly SourceCache<ItemValue, int> _items = new SourceCache<ItemValue, int>(v => v.Id);

        public IObservable<IChangeSet<ItemValue, int>> Connect() => _items.Connect();
        public VerificationTestPointViewModel()
        {
        }

        [Reactive] public int TestNumber { get; set; }

        public ICollection<VerificationViewModel> TestsCollection { get; set; } = new List<VerificationViewModel>();

        public PressureFactorViewModel Pressure => this.GetPressureTest();

        public TemperatureFactorViewModel Temperature => this.GetTemperatureTest();

        public SuperFactorViewModel SuperFactor => this.GetSuperFactorTest();

        public VolumeViewModelBase Volume => this.GetVolumeTest();

        public void UpdateItemValues(ICollection<ItemValue> itemValues)
        {
            _items.Edit(update => update.AddOrUpdate(itemValues));

          
        }

        public void AddTest(VerificationViewModel test)
        {

        }
    }
}