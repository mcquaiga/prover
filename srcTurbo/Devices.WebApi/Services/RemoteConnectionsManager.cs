using Devices.Communications;
using Devices.Communications.Interfaces;
using Devices.Communications.IO;
using Devices.Core.Interfaces;
using Devices.WebApi.Responses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace Devices.WebApi.Services
{
    public class RemoteConnection
    {
        public ICommunicationsClient Client { get; }

        public IDeviceWithValues Device { get; }

        public Guid Id { get; set; }

        public RemoteConnection(ICommunicationsClient client, IDeviceWithValues device)
           : this(Guid.NewGuid(), client, device)
        {
        }

        public RemoteConnection(Guid id, ICommunicationsClient client, IDeviceWithValues device)
        {
            Id = id;
            Client = client;
            Device = device;
        }

        public string GetPortName()
        {
            return Client.CommPort.Name;
        }
    }

    public class RemoteConnectionsManager
    {
        public IConnectableObservable<RemoteConnection> ConnectionStatusObservable { get; }

        public RemoteConnectionsManager()
        {
            _connCompletedObservable = new Subject<RemoteConnection>();
            _connCompletedObservable
                .Subscribe(x => Console.WriteLine($"Connection completed for session id {x}"));

            ConnectionStatusObservable = _connCompletedObservable.Publish();
            ConnectionStatusObservable.Connect();
        }

        public async Task<RemoteConnection> EndSession(Guid id)
        {
            var session = Get(id);
            await session.Client.Disconnect();
            session.Client.CommPort.Dispose();

            if (_sessions.Remove(id, out var removedSession))
                return removedSession;
            else
                throw new KeyNotFoundException("Session could not be removed from the queue.");
        }

        public RemoteConnection Get(Guid id)
        {
            if (_sessions.TryGetValue(id, out var result))
            {
                return result;
            }

            throw new KeyNotFoundException($"Session id: {id} could not be found");
        }

        public IEnumerable<ConnectionGet> Get()
        {
            return _sessions.ToList().Select(kv => new ConnectionGet(kv.Key, kv.Value.Device));
        }

        public async Task<Guid> StartSession(IDevice device, string portName, IObserver<string> status)
        {
            if (!IsPortAvailable(portName))
                throw new UnauthorizedAccessException($"{portName} is in use by another session. Please try a different port.");

            var id = Guid.NewGuid();

            var commPort = new SerialPort(portName, 9600);

            Observable.StartAsync(async () =>
                {
                    var conn = await Connect(id);
                    _sessions.TryAdd(id, conn);
                    return conn;
                },
                NewThreadScheduler.Default)
            .Subscribe(x => _connCompletedObservable.OnNext(x));

            return id;

            async Task<RemoteConnection> Connect(Guid sessionId)
            {
                var conn = await DeviceConnection.ConnectAsync(device, commPort, statusObserver: status);
                var evc = await conn.GetDeviceAsync();

                var tup = new RemoteConnection(id, conn, evc);

                return tup;
            }
        }

        private readonly ConcurrentDictionary<Guid, RemoteConnection> _sessions
                                            = new ConcurrentDictionary<Guid, RemoteConnection>();

        private ISubject<RemoteConnection> _connCompletedObservable;

        private bool IsPortAvailable(string portName)
        {
            return !_sessions.Any(kv => string.Equals(kv.Value.GetPortName(), portName, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}