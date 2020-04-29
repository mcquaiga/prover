using Devices.Core.Items;
using DynamicData;
using Prover.Application.ViewModels.Corrections;
using Prover.Application.ViewModels.Volume;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;

namespace Prover.Application.ViewModels
{
    public class VerificationTestPointViewModel : VerificationViewModel, IAssertVerification
    {
        private readonly SourceCache<ItemValue, int> _items = new SourceCache<ItemValue, int>(v => v.Id);

        private VerificationTestPointViewModel(int testNumber, bool verified) : base() =>
            TestNumber = testNumber;

        public VerificationTestPointViewModel()
        {
        }

        public int TestNumber { get; set; }

        public ICollection<VerificationViewModel> VerificationTests { get; set; } = new List<VerificationViewModel>();

        public PressureFactorViewModel Pressure => VerificationTests.OfType<PressureFactorViewModel>().FirstOrDefault();

        public TemperatureFactorViewModel Temperature => VerificationTests.OfType<TemperatureFactorViewModel>().FirstOrDefault();

        public SuperFactorViewModel SuperFactor => VerificationTests.OfType<SuperFactorViewModel>().FirstOrDefault();

        public VolumeViewModelBase Volume => VerificationTests.OfType<VolumeViewModelBase>().FirstOrDefault();

        public void AddTest(VerificationViewModel test)
        {
            VerificationTests.Add(test);
        }

        public IObservable<IChangeSet<ItemValue, int>> Connect() => _items.Connect();

        public void Initialize(ICollection<VerificationViewModel> tests = null)
        {
            tests?.ForEach(VerificationTests.Add);
            RegisterVerificationsForVerified(VerificationTests);
        }

        //public void UpdateItemValues(DeviceType deviceType, ICollection<ItemValue> itemValues)
        //{
        //    this.UpdateValues(itemValues, deviceType);

        //    foreach (var correction in this.GetCorrectionTests())
        //    {
        //        var itemType = correction.GetProperty(nameof(CorrectionTestViewModel<IItemGroup>.Items));

        //        itemType?.SetValue(correction, deviceType.GetGroupValues(itemValues, itemType.PropertyType));
        //    }

        //    _items.Edit(update => update.AddOrUpdate(itemValues));
        //}

        protected override void Dispose(bool isDisposing)
        {
            VerificationTests.ForEach(t => t.DisposeWith(Cleanup));
            VerificationTests.Clear();
        }
    }
}