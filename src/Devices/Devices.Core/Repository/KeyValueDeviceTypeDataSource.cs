﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Devices.Core.Interfaces;
using Devices.Core.Items;
using Prover.Shared.Interfaces;

namespace Devices.Core.Repository
{
    public class KeyValueDeviceTypeDataSource : IDeviceTypeCacheSource<DeviceType>
    {
        private readonly IRepository<DeviceType> _keyValueRepository;

        public KeyValueDeviceTypeDataSource(IRepository<DeviceType> keyValueRepository)
        {
            _keyValueRepository = keyValueRepository;
        }

        public IObservable<DeviceType> GetDeviceTypes()
        {
            return _keyValueRepository.GetAll().ToObservable();
        }

        public IObservable<ItemMetadata> GetItems()
        {
            throw new NotImplementedException();
        }

        public void Save(IEnumerable<DeviceType> deviceTypes)
        {
            try
            {
                deviceTypes.ToList().ForEach(d => { _keyValueRepository.Update(d); });
            }
            catch (AggregateException aggEx)
            {
                foreach (var ex in aggEx.InnerExceptions)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
          
        }
    }
}