﻿using System.Linq;
using System.Reactive.Disposables;
using System.Windows;
using Client.Desktop.Wpf.Extensions;
using ReactiveUI;

namespace Client.Desktop.Wpf.Views.Verifications
{
    /// <summary>
    ///     Interaction logic for VerificationTest.xaml
    /// </summary>
    public partial class QaTestRunView
    {
        public QaTestRunView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.OneWayBind(ViewModel, vm => vm.TestManager, view => view.NewTestSettingsSection.Visibility,
                        value => value == null ? Visibility.Visible : Visibility.Collapsed)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.TestManager, v => v.EditTestToolBarControl.Visibility,
                        value => value != null ? Visibility.Visible : Visibility.Collapsed)
                    .DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.TestManager, view => view.TestManagerView.ViewModel).DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.StartTestCommand, v => v.StartTestButton).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.DeviceTypes, v => v.DeviceTypes.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedDeviceType, v => v.DeviceTypes.SelectedItem).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.CommPorts, v => v.CommPorts.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedCommPort, v => v.CommPorts.SelectedItem).DisposeWith(d);

                this.OneWayBind(ViewModel, vm => vm.BaudRates, v => v.BaudRates.ItemsSource,
                        value => value.Select(x => x.ToString()))
                    .DisposeWith(d);

                this.Bind(ViewModel, vm => vm.SelectedBaudRate, v => v.BaudRates.SelectedItem, vmp => vmp.ToString(),
                        vp => string.IsNullOrEmpty(vp?.ToString()) ? 0 : int.Parse(vp.ToString()))
                    .DisposeWith(d);

                //Tach Settings
                this.OneWayBind(ViewModel, vm => vm.CommPorts, v => v.TachCommPorts.ItemsSource).DisposeWith(d);
                this.Bind(ViewModel, vm => vm.SelectedTachCommPort, v => v.TachCommPorts.SelectedItem).DisposeWith(d);

                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.PrintTestReport, v => v.PrintButton).DisposeWith(d);
                this.BindCommand(ViewModel, vm => vm.SubmitTest, v => v.SubmitTestButton).DisposeWith(d);
                
                this.BindCommand(ViewModel, vm => vm.LoadFromFile, v => v.LoadFromFileButton).DisposeWith(d);

                this.CleanUpDefaults().DisposeWith(d);
            });
        }
    }
}