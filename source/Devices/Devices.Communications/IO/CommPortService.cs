//using System;
//using System.Reactive.Linq;
//using System.Reactive.Subjects;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Devices.Communications.IO
//{
//    public class CommPortService : IDisposable
//    {
//        public ICommPort CommPort { get; }
//        private readonly ISubject<string> _dataSent = new Subject<string>();
//        private IDisposable _dataReceivedConnection;
//        private IDisposable _dataReceived;

//        public CommPortService(ICommPort commPort)
//        {
//            CommPort = commPort;
//        }

//        public void Open(CancellationToken ct)
//        {
//            if (IsOpen()) return;

//            DataReceived = CommPort.DataReceivedObservable().Publish();
//            _dataReceived = DataReceived.Subscribe();
//            _dataReceivedConnection = DataReceived.Connect();

//            CommPort.Open(ct);
//        }

//        public bool IsOpen()
//        {
//            return CommPort.IsOpen();
//        }

//        public void Close()
//        {
//            _dataReceived.Dispose();
//            _dataReceivedConnection.Dispose();
//            CommPort.Close();
//        }

//        public async Task Send(string data)
//        {
//            _dataSent.OnNext(data);
//            await CommPort.Send(data);
//        }


//        public IConnectableObservable<char> DataReceived { get; private set; }
//        public IObservable<string> DataSent => _dataSent;

//        public void Dispose()
//        {
//            CommPort.Close();
//            CommPort.Dispose();
//            _dataReceived.Dispose();
//            _dataReceivedConnection.Dispose();
//            _dataSent.OnCompleted();
//        }
//    }
//}