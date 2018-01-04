using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Messaging;
using Prover.Core.ExternalDevices.DInOutBoards;

namespace Prover.Core.Communication
{
    public class TachometerService : IDisposable
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        private readonly IDInOutBoard _outputBoard;
        private readonly SerialPort _serialPort;

        public TachometerService(string portName, IDInOutBoard outputBoard)
        {            
            _serialPort = null;
            
            if (SerialPort.GetPortNames().Contains(portName))            
            {
                _serialPort = new SerialPort(portName, 9600);
            }

            _outputBoard = outputBoard;
        }

        public void Dispose()
        {
            _serialPort?.Close();
            _serialPort?.Dispose();
            _outputBoard?.Dispose();
        }

        public async Task ResetTach()
        {
            if (_serialPort != null)
            {
                if (!_serialPort.IsOpen())
                    await _serialPort.Open(new CancellationToken());

                await _serialPort.Send($"@T1{(char)13}");
                Thread.Sleep(50);
                await _serialPort.Send($"6{(char)13}");
            }
                        
            await Task.Run(() =>
            {
                _outputBoard?.StartMotor();
                Thread.Sleep(500);
                _outputBoard?.StopMotor();
                Thread.Sleep(100);
            });

            Thread.Sleep(2000);
        }

        public async Task<int> ReadTach()
        {
            var cts = new CancellationTokenSource(2000);           
            
            if (_serialPort == null)
                return -1;
            
            try
            {
                if (!_serialPort.IsOpen())
                    await _serialPort.Open(cts.Token);

                var response = new LineProcessor().ResponseObservable(_serialPort.DataReceivedObservable)
                    .Timeout(TimeSpan.FromSeconds(5))
                    .FirstAsync()
                    .PublishLast();

                using (response.Connect())
                {
                    await _serialPort.Send("@D0");
                    await _serialPort.Send(((char)13).ToString());

                    var result = await response;

                    Log.Debug($"Read data from Tach: {result}");
                    return ParseTachValue(result);
                }
            }
            finally
            {
                await _serialPort.Close();
            }
           
        }

        public static int ParseTachValue(string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;

            const string pattern = @"(\d+)";
            int result;
            value = value.Replace("\n", " ").Replace("\r", " ");
            value = value.Substring(value.IndexOf("D0", StringComparison.Ordinal) + 2);
            var regEx = new Regex(pattern, RegexOptions.IgnoreCase);
            if (int.TryParse(regEx.Match(value).Value, out result))
                return result;
            return -1;
        }
    }

    //public class TachometerResponseProcessor : ResponseProcessor<long>
    //{
    //    public override IObservable<long> ResponseObservable(IObservable<char> source)
    //    {
    //        return Observable.Create<long>(observer =>
    //        {
    //            var msgChars = new List<char>();

    //            Action emitPacket = () =>
    //            {
    //                if (msgChars.Count > 0)
    //                {
    //                    var msg = string.Concat(msgChars.ToArray());

    //                    //observer.OnNext(msg);
    //                    msgChars.Clear();
    //                }
    //            };

    //            return source.Subscribe(
    //                c =>
    //                {
    //                    //if (c) emitPacket();

    //                    msgChars.Add(c);

    //                    if (c.IsAcknowledgement())
    //                        emitPacket();

    //                    if (c.IsEndOfTransmission())
    //                        emitPacket();
    //                },
    //                observer.OnError,
    //                () =>
    //                {
    //                    emitPacket();
    //                    observer.OnCompleted();
    //                });
    //        });
    //    }
    //}
}