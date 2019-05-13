using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Devices.Mobile.Services;
using Devices.Mobile.Views;
using Xamarin.Essentials;

namespace Devices.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            DependencyService.Register<DeviceService>();
            DependencyService.Register<DeviceSignalRClient>();

            MainPage = new MainPage();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }
    }
}