using System;
using System.Reactive.Subjects;
using Devices.Core.Items;
using Newtonsoft.Json;

namespace Prover.Application.Services.LiveReadCorrections
{
    [Serializable]
    public class ItemLiveReadStatus
    {
        [NonSerialized]
        private readonly Subject<ItemLiveReadStatus> _statusSubject = new Subject<ItemLiveReadStatus>();

        public ItemLiveReadStatus(ItemMetadata item, AveragedReadingStabilizer stabilizer)
        {
            Item = item;
            Stabilizer = stabilizer;
            Value = ItemValue.Create(Item, 0m);
        }

        public IObservable<ItemLiveReadStatus> StatusUpdates => _statusSubject;

        public ItemMetadata Item { get; }

        public AveragedReadingStabilizer Stabilizer { get; }

        public decimal TargetValue => Stabilizer.TargetValue;

        public ItemValue Value { get; private set; }

        public void AddUpdate(ItemValue value)
        {
            Value = value;
            if (value.DecimalValue().HasValue)
            {
                Stabilizer.Add(value.DecimalValue() ?? 0m);
                _statusSubject.OnNext(this);
            }
        }
    }
}