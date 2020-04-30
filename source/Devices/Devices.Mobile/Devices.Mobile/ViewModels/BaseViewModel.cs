using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

using Devices.Mobile.Models;
using Devices.Mobile.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Devices.Mobile.ViewModels
{
    public class BaseViewModel : ReactiveObject
    {
        public static DeviceService DeviceService => DeviceService.Instance;
        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>() ?? new MockDataStore();

        [Reactive]
        public bool IsBusy { get; set; }

        [Reactive]
        public string Title { get; set; }

        public BaseViewModel()
        {
        }

        private bool isBusy = false;
        private string title = string.Empty;
    }
}