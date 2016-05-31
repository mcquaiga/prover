using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.Practices.Unity;
using NLog;
using Prover.Core.Events;
using Prover.Core.Models;
using Prover.Core.Models.Instruments;
using Prover.SerialProtocol;
using LogManager = NLog.LogManager;

namespace Prover.Core.Communication
{
    //public class InstrumentCommunicator
    //{
    //    private readonly miSerialProtocolClass _miSerial;
    //    private readonly Logger _log = LogManager.GetCurrentClassLogger();
    //    private readonly int maxConnectAttempts = 10;

    //    public InstrumentCommunicator(IUnityContainer container, ICommPort commPort,
    //        InstrumentType instrumentType)
    //    {
    //        _eventAggregator = eventAggregator;

    //        switch (instrumentType)
    //        {
    //            case InstrumentType.Ec300:
    //                _miSerial = new EC300Class(commPort);
    //                break;
    //            default:
    //                _miSerial = new MiniMaxClass(commPort);
    //                break;
    //        }

    //        IsConnected = false;
    //    }

    //    public InstrumentCommunicator(IEventAggregator eventAggregator, ICommPort commPort, Instrument instrument)
    //        : this(eventAggregator, commPort, instrument.Type)
    //    {
    //    }

    //    public bool IsConnected { get; private set; }

    //    public async Task<Dictionary<int, string>> DownloadItemsAsync(IEnumerable<ItemDetail> itemsToDownload,
    //        bool disconnectAfter = true)
    //    {
    //        await Connect();

    //        var result = await Task.Run(() => DownloadItems(itemsToDownload));

    //        if (disconnectAfter)
    //            await Disconnect();

    //        return result;
    //    }

    //    public async Task<string> DownloadItem(int itemNumber, bool disconnectAfter = true)
    //    {
    //        await Connect();

    //        var result = await Task.Run(() => _miSerial.RD(itemNumber));

    //        if (disconnectAfter)
    //            await Disconnect();

    //        return Convert.ToString(result);
    //    }

    //    public async Task WriteItem(int itemNumber, string value, bool disconnectAfter = true)
    //    {
    //        await Connect();

    //        await Task.Run(() => _miSerial.WD(itemNumber, value));

    //        if (disconnectAfter)
    //            await Disconnect();
    //    }

    //    private Dictionary<int, string> DownloadItems(IEnumerable<ItemDetail> itemsToDownload)
    //    {
    //        var myItems = _miSerial.RG((from i in itemsToDownload select i.Number).ToList());
    //        return myItems;
    //    }

    //    public async Task Connect()
    //    {
    //        var tryCount = 0;

    //        do
    //        {
    //            try
    //            {
    //                if (!IsConnected)
    //                {
    //                    await
    //                        _eventAggregator.PublishOnUIThreadAsync(
    //                            new ConnectionStatusEvent(ConnectionStatusEvent.Status.Connecting, tryCount + 1,
    //                                maxConnectAttempts));
    //                    await Task.Run(() => _miSerial.Connect()).ContinueWith(taskResult =>
    //                    {
    //                        if (!taskResult.IsFaulted)
    //                            IsConnected = true;
    //                        else
    //                            IsConnected = false;
    //                    });
    //                }
    //            }
    //            catch (AggregateException ae)
    //            {
    //                ae.Handle(x =>
    //                {
    //                    if (x is CommExceptions)
    //                    {
    //                        _log.Error("An error occured connecting to instrumnet.", x);
    //                        return true;
    //                    }

    //                    return false;
    //                });
    //                tryCount++;
    //                IsConnected = false;
    //            }
    //        } while (!IsConnected && tryCount < maxConnectAttempts);

    //        if (tryCount >= maxConnectAttempts)
    //        {
    //            _log.Error("Could not connect to instrument.");
    //            throw new InstrumentCommunicationException(InstrumentErrorsEnum.TooManyRetransmissionsError);
    //        }
    //        await _eventAggregator.PublishOnUIThreadAsync(
    //            new ConnectionStatusEvent(ConnectionStatusEvent.Status.Connected,
    //                tryCount, maxConnectAttempts));
    //    }

    //    public async Task Disconnect()
    //    {
    //        if (IsConnected)
    //            await Task.Run(() => _miSerial.Disconnect());

    //        IsConnected = false;

    //        await _eventAggregator.PublishOnUIThreadAsync(
    //            new ConnectionStatusEvent(ConnectionStatusEvent.Status.Disconnected,
    //                0, maxConnectAttempts));
    //    }

    //    public async Task<decimal> LiveReadItem(int itemNumber)
    //    {
    //        return await Task.Run(async () =>
    //        {
    //            await Connect();
    //            return Convert.ToDecimal(_miSerial.LR(itemNumber));
    //        });
    //    }
    //}
}