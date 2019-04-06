using Autofac;
using Caliburn.Micro;
using NLog;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.Common.Models;
using Prover.CommProtocol.Common.Models.Instrument;
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

        public IObserver<string> Status { get; } = new Subject<string>();

        public RotaryStressTest(ISettingsService testSettings)
        {
            _testSettings = testSettings;
        }

        public async Task Run(IEvcDevice instrumentType, ICommPort commPort, Client client, CancellationToken ct = new CancellationToken())
        {
            try
            {
                _testSettings.Local.AutoSave = false;

                var meterTypes = (IEnumerable<MeterIndexItemDescription>)instrumentType.ItemsMetadata.GetItemDescriptions(433);
                
                IEnumerable<ItemValue> items;
                using(var commClient = EvcCommunicationClient.Create(instrumentType, commPort))
                {
                    await commClient.Connect(ct);
                    items = await commClient.GetAllItems(); 
                    await commClient.Disconnect();
                }          
                
                var mt = items.GetItem(433).ItemDescription as MeterIndexItemDescription;
                
                foreach (var meter in meterTypes.Where(m => m.MountType == mt.MountType))
                {
                    using(var commClient = EvcCommunicationClient.Create(instrumentType, commPort))
                    {
                        commClient.Status.Subscribe(Status);
                        await commClient.Connect(ct);

                        await commClient.SetItemValue(432, meter.MeterDisplacement.Value.ToString());
                        await commClient.SetItemValue(433, GetMeterId(instrumentType, meter));

                        await commClient.Disconnect();
                    }                    

                    using(var qaRunTestManager = IoC.Get<IQaRunTestManager>())
                    {
                        await qaRunTestManager.InitializeTest(instrumentType, commPort, _testSettings, ct, client, Status);
                        await qaRunTestManager.RunCorrectionTest(0, ct);
                        await qaRunTestManager.RunVolumeTest(ct);
                        await qaRunTestManager.SaveAsync();
                    }                    
                }
            }
            catch(Exception ex)
            {
                _log.Error(ex);
            }
            finally
            {
                await _testSettings.RefreshSettings();
            }
           
        }

        private int GetMeterId(IEvcDevice instrumentType, MeterIndexItemDescription meterInfo)
        {
            if (instrumentType == HoneywellInstrumentTypes.Ec350)
            {
                return meterInfo.Ids.FirstOrDefault(i => i > 80);
            }

            return meterInfo.Ids.FirstOrDefault(i => i < 80);
        }
    }
}
