
//using Devices.Core.Items;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Devices.Core
//{
//    public abstract class DeviceTypeBase : IDeviceType
//    {
//        public virtual int AccessCode { get; set; }

//        public virtual bool? CanUseIrDaPort { get; set; }

//        public virtual int Id { get; set; }

//        public virtual bool IsHidden { get; set; }

//        public virtual ICollection<ItemMetadata> Items { get; set; }

//        public IObservable<ItemMetadata> ItemsObservable { get; }

//        public virtual int? MaxBaudRate { get; set; }

//        public virtual string Name { get; set; }

//    }
//}