using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Instruments;
using Prover.CommProtocol.Common.Items;
using Prover.Domain.Models.TestRuns;
using Prover.Shared.Enums;

namespace Prover.Domain.QaTestRun
{
    public interface IQaRunTestManager : IDisposable
    {
        IInstrument Instrument { get; }

        IObservable<string> TestStatus { get; }

        Task RunTest(TestLevel level, CancellationToken ct = new CancellationToken());
        Task SaveAsync();
        Task RunVerifiers();
    }

    public class TestRunManager : IQaRunTestManager
    {
        protected static Logger Log = LogManager.GetCurrentClassLogger();
        //private readonly IProverStore<Instrument> _instrumentStore;
        //private readonly IEnumerable<PostTestResetBase> _postTestCommands;
        //private readonly IReadingStabilizer _readingStabilizer;
        private readonly Subject<string> _testStatus = new Subject<string>();
        //private readonly IEnumerable<PreTestValidationBase> _validators;

        public TestRunManager()
        {
           
        }

        public async Task CreateTestRun(IInstrument instrument)
        {
            Instrument = instrument;
            TestRun = TestRun.Create(Instrument);

            if (Instrument.DriveType == DriveTypeDescripter.Rotary)
            {
                
            }
        }

        //public VolumeTestManagerBase VolumeTestManager { get; set; }

        public IObservable<string> TestStatus => _testStatus.AsObservable();

        public IInstrument Instrument { get; private set; }

        public TestRun TestRun { get; private set; }

        public async Task RunTest(TestLevel level, CancellationToken ct)
        {
            try
            {
                var testPoint = TestRun.TestPoints[level];
                if (Instrument == null) throw new NullReferenceException("Call InitializeTest before runnning a test");

                _testStatus.OnNext($"Stabilizing live readings...");
                //await _readingStabilizer.WaitForReadingsToStabilizeAsync(_communicationClient, Instrument, level, ct);

                _testStatus.OnNext($"Downloading items...");
                await GetTemperatureItems(testPoint.Temperature);
                await GetPressureItems(testPoint.Pressure);

                if (TestRun.TestPoints[level].Volume != null)
                {
                    _testStatus.OnNext($"Running volume test...");
                    //await VolumeTestManager.RunTest(_communicationClient, Instrument.VolumeTest, ct);

                    //Execute any Post test clean up methods
                    //foreach (var command in _postTestCommands)
                    //    await command.Execute(_communicationClient, Instrument, _testStatus);
                }

                _testStatus.OnNext($"Saving test...");
                await SaveAsync();
            }
            catch (OperationCanceledException ex)
            {
                Log.Info("Test run cancelled.");
            }
        }

        private async Task GetPressureItems(IPressureItems testPointPressure)
        {
            if (testPointPressure == null) return;

            var evcData = await Instrument.DownloadPressureItems();
            testPointPressure.Update(evcData);
        }

        private async Task GetTemperatureItems(ITemperatureItems tempTest)
        {
            if (tempTest == null) return;
            var evcData = await Instrument.DownloadTemperatureItems();
            tempTest.Update(evcData);
        }

        public async Task SaveAsync()
        {
            //try
            //{
            //    await _instrumentStore.UpsertAsync(Instrument);
            //}
            //catch (Exception ex)
            //{
            //    Log.Error(ex);
            //}
        }

        public async Task RunVerifiers()
        {
            //if (_validators != null && _validators.Any())
            //    foreach (var validator in _validators)
            //    {
            //        _testStatus.OnNext($"Verifying items...");
            //        await validator.Execute(_communicationClient, Instrument);
            //    }
        }

        public void Dispose()
        {
            //_communicationClient.Dispose();
            //VolumeTestManager.Dispose();
        }

        //public async Task DownloadTemperatureTestItems(int levelNumber)
        //{
        //    var firstOrDefault = Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == levelNumber);
        //    var test = firstOrDefault?.TemperatureTest;

        //    if (test != null)
        //        test.Items =
        //            (ICollection<ItemValue>)
        //            await _communicationClient.GetItemValues(_communicationClient.ItemDetails.TemperatureItems());
        //}

        //public async Task DownloadPressureTestItems(int level)
        //{
        //    var firstOrDefault = Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == level);
        //    var test = firstOrDefault?.PressureTest;
        //    if (test != null)
        //        test.Items =
        //            (ICollection<ItemValue>)
        //            await _communicationClient.GetItemValues(_communicationClient.ItemDetails.PressureItems());
        //}
    }
}