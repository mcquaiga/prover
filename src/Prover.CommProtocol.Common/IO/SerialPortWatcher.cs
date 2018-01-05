using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace Prover.CommProtocol.Common.IO
{
    /// <summary>
    /// Make sure you create this watcher in the UI thread if you are using the com port list in the UI
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public sealed class SerialPortWatcher : IDisposable
    {
        public SerialPortWatcher()
        {
            _taskScheduler = TaskScheduler.Default;
            ComPorts = new ObservableCollection<string>(System.IO.Ports.SerialPort.GetPortNames().OrderBy(s => s));

            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent");

            _watcher = new ManagementEventWatcher(query);
            _watcher.EventArrived += (sender, eventArgs) => CheckForNewPorts(eventArgs);
            _watcher.Start();
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

            foreach (string comPort in ComPorts)
            {
                if (!ports.Contains(comPort))
                {
                    ComPorts.Remove(comPort);
                }
            }

            foreach (var port in ports)
            {
                if (!ComPorts.Contains(port))
                {
                    AddPort(port);
                }
            }
        }

        private void AddPort(string port)
        {
            for (int j = 0; j < ComPorts.Count; j++)
            {
                if (port.CompareTo(ComPorts[j]) < 0)
                {
                    ComPorts.Insert(j, port);
                    break;
                }
            }

        }

        public ObservableCollection<string> ComPorts { get; private set; }

        #region IDisposable Members

        public void Dispose()
        {
            _watcher.Stop();
        }

        #endregion

        private ManagementEventWatcher _watcher;
        private TaskScheduler _taskScheduler;
    }

    //private static List<string> _commPorts;
    //public static IObservable<string[]> PortsWatcher()
    //{
    //    return Observable.Create<string[]>(observer =>
    //    {
    //        return Observable
    //            .Interval(TimeSpan.FromSeconds(1))
    //            .Subscribe(
    //                _ =>
    //                {
    //                    var ports = System.IO.Ports.SerialPort.GetPortNames().ToList();
    //                    if (!_commPorts.SequenceEqual(ports))
    //                        observer.OnNext(ports.ToArray());
    //                });
    //    });
    //}
}