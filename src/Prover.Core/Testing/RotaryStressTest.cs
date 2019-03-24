using Autofac;
using Caliburn.Micro;
using NLog;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.Common.Models;
using Prover.CommProtocol.MiHoneywell;
using Prover.Core.Models.Clients;
using Prover.Core.Settings;
using Prover.Core.VerificationTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LogManager = NLog.LogManager;

namespace Prover.Core.Testing
{
    public class RotaryStressTest
    {
        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly ISettingsService _testSettings;
        private readonly Func<string, int, ICommPort> _commPortFactory;

        public IObserver<string> Status { get; } = new Subject<string>();

        public RotaryStressTest(ISettingsService testSettings, Func<string, int, ICommPort> commPortFactory)
        {
            _testSettings = testSettings;
            _commPortFactory = commPortFactory;
        }

        public async Task Run(InstrumentType instrumentType, Client client, CancellationToken ct = new CancellationToken())
        {
            _testSettings.Local.AutoSave = false;
            _testSettings.TestSettings.StabilizeLiveReadings = false;

            try
            {
                await RunMechanicalTest(instrumentType, client, ct);
                //await RunRotaryTest(instrumentType, client, ct);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            finally
            {
                await _testSettings.RefreshSettings();
            }
           
        }

        private async Task RunMechanicalTest(InstrumentType instrumentType, Client client, CancellationToken ct)
        {
            IEnumerable<ItemValue> items;
            var corrVolumeUnits = instrumentType.ItemsMetadata.GetItemDescriptions(90);

            var commPort = GetCommPort();
            using (var commClient = EvcCommunicationClient.Create(instrumentType, commPort))
            {
                await commClient.Connect(ct);
                items = await commClient.GetAllItems();
                await commClient.Disconnect();
            }

            foreach (var corUnits in corrVolumeUnits)
            {
                commPort = GetCommPort();
                using (var commClient = EvcCommunicationClient.Create(instrumentType, commPort))
                {
                    commClient.Status.Subscribe(Status);
                    await commClient.Connect(ct);

                    await commClient.SetItemValue(90, corUnits.Id);                

                    await commClient.Disconnect();
                }

                commPort = GetCommPort();
                using (var qaRunTestManager = IoC.Get<IQaRunTestManager>())
                {
                    await qaRunTestManager.InitializeTest(instrumentType, commPort, _testSettings, ct, client, false);
                    qaRunTestManager.Status.Subscribe(Status);
                    await qaRunTestManager.RunCorrectionTest(0, ct);
                    await qaRunTestManager.RunVolumeTest(ct);
                    await qaRunTestManager.SaveAsync();
                }
            }
        }

        private async Task RunRotaryTest(InstrumentType instrumentType, Client client, CancellationToken ct)
        {
            IEnumerable<ItemValue> items;
            var meterTypes = instrumentType.ItemsMetadata.GetItemDescriptions(432);

            var commPort = GetCommPort();
            using (var commClient = EvcCommunicationClient.Create(instrumentType, commPort))
            {
                await commClient.Connect(ct);
                items = await commClient.GetAllItems();
                await commClient.Disconnect();
            }

            var mt = items.GetItem(432).ItemDescription as MeterIndexItemDescription;

            foreach (MeterIndexItemDescription meter in meterTypes.Where(m => (m as MeterIndexItemDescription).MountType == mt.MountType))
            {
                commPort = GetCommPort();
                using (var commClient = EvcCommunicationClient.Create(instrumentType, commPort))
                {
                    commClient.Status.Subscribe(Status);
                    await commClient.Connect(ct);

                    await commClient.SetItemValue(432, GetMeterId(instrumentType, meter));
                    await commClient.SetItemValue(439, meter.MeterDisplacement.Value.ToString());

                    await commClient.Disconnect();
                }

                commPort = GetCommPort();
                using (var qaRunTestManager = IoC.Get<IQaRunTestManager>())
                {
                    await qaRunTestManager.InitializeTest(instrumentType, commPort, _testSettings, ct, client);
                    qaRunTestManager.Status.Subscribe(Status);
                    await qaRunTestManager.RunCorrectionTest(0, ct);
                    await qaRunTestManager.RunVolumeTest(ct);
                    await qaRunTestManager.SaveAsync();
                }
            }
        }

        private ICommPort GetCommPort()
        {
            return _commPortFactory.Invoke(_testSettings.Local.InstrumentCommPort, _testSettings.Local.InstrumentBaudRate);
        }

        private int GetMeterId(InstrumentType instrumentType, MeterIndexItemDescription meterInfo)
        {
            if (instrumentType.Name == "EC-350")
            {
                return meterInfo.Ids.FirstOrDefault(i => i > 80);
            }

            return meterInfo.Ids.FirstOrDefault(i => i < 80);
        }
    }
}
