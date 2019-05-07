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
    public class RemoteConnectionsManager
    {
        public IObservable<Tuple<ICommunicationsClient, IDeviceWithValues>> ConnectionCompleteStream { get; }

        public RemoteConnectionsManager(ICommPort commPort)
        {
            _connCompletedObservable = new Subject<Guid>();
            _connCompletedObservable
                .Subscribe(x => Console.WriteLine($"Connection completed for session id {x}"));
        }

        public ConnectionGet Add(ICommunicationsClient client, IDeviceWithValues device)
        {
            var tup = new Tuple<ICommunicationsClient, IDeviceWithValues>(client, device);
            var id = Guid.NewGuid();

            _sessions.TryAdd(id, tup);

            return new ConnectionGet(id, device);
        }

        public Tuple<ICommunicationsClient, IDeviceWithValues> Get(Guid id)
        {
            if (_sessions.TryGetValue(id, out var result))
            {
                return result;
            }

            throw new KeyNotFoundException($"Session id: {id} could not be found");
        }

        public async Task<Guid> StartSession(IDevice device, ICommPort commPort)
        {
            var id = Guid.NewGuid();

            Observable.StartAsync(async () => _sessions.TryAdd(id, await Connect()) ? id : Guid.Empty, NewThreadScheduler.Default)
                        .Subscribe(x => _connCompletedObservable.OnNext(x));

            async Task<Tuple<ICommunicationsClient, IDeviceWithValues>> Connect()
            {
                var conn = await DeviceConnection.ConnectAsync(device, commPort);
                var evc = await conn.GetDeviceAsync();

                var tup = new Tuple<ICommunicationsClient, IDeviceWithValues>(conn, evc);

                return tup;
            }

            return id;
        }

        private readonly ConcurrentDictionary<Guid, Tuple<ICommunicationsClient, IDeviceWithValues>> _sessions
                                            = new ConcurrentDictionary<Guid, Tuple<ICommunicationsClient, IDeviceWithValues>>();

        private ISubject<Guid> _connCompletedObservable;
    }
}