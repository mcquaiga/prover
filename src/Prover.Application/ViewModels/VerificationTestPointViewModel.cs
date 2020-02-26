using System;
using System.Collections.Generic;
using Devices.Core.Items;
using DynamicData;
using Prover.Application.Extensions;
using Prover.Application.ViewModels.Corrections;
using Prover.Application.ViewModels.Volume;
using ReactiveUI.Fody.Helpers;

namespace Prover.Application.ViewModels
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