using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace Prover.InstrumentProtocol.Core.IO
{
    /// <summary>
    ///     Make sure you create this watcher in the UI thread if you are using the com port list in the UI
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class SerialPortWatcher : IDisposable
    {
        private readonly TaskScheduler _taskScheduler;

        private readonly ManagementEventWatcher _watcher;

        public SerialPortWatcher()
        {
            _taskScheduler = TaskScheduler.Default;
            ComPorts = new ObservableCollection<string>(System.IO.Ports.SerialPort.GetPortNames().OrderBy(s => s));

            var query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent");

            _watcher = new ManagementEventWatcher(query);
            _watcher.EventArrived += (sender, eventArgs) => CheckForNewPorts(eventArgs);
            _watcher.Start();
        }

        public ObservableCollection<string> ComPorts { get; }

        public void Dispose()
        {
            _watcher.Stop();
        }

        private void AddPort(string port)
        {
            for (var j = 0; j < ComPorts.Count; j++)
                if (port.CompareTo(ComPorts[j]) < 0)
                {
                    ComPorts.Insert(j, port);
                    break;
                }
        }

        private void CheckForNewPorts(EventArrivedEventArgs args)
        {
            // do it async so it is performed in the UI thread if this class has been created in the UI thread
            Task.Factory.StartNew(CheckForNewPortsAsync, CancellationToken.None, TaskCreationOptions.None,
                _taskScheduler);
        }

        private void CheckForNewPortsAsync()
        {
            IEnumerable<string> ports = System.IO.Ports.SerialPort.GetPortNames().OrderBy(s => s);

            foreach (var comPort in ComPorts)
                if (!ports.Contains(comPort))
                    ComPorts.Remove(comPort);

            foreach (var port in ports)
                if (!ComPorts.Contains(port))
                    AddPort(port);
        }
    }
}