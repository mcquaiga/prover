﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Newtonsoft.Json;
using NLog;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.IO;
using Prover.CommProtocol.Common.Items;
using Prover.CommProtocol.MiHoneywell.Items;
using Prover.Core.Communication;
using Prover.Core.Models.Clients;
using Prover.Core.Models.Instruments;
using Prover.Core.Models.Instruments.DriveTypes;
using Prover.Core.Services;
using Prover.Core.Settings;
using Prover.Core.Shared.Enums;
using Prover.Core.VerificationTests.TestActions;
using Prover.Core.VerificationTests.VolumeVerification;
using LogManager = NLog.LogManager;

namespace Prover.Core.VerificationTests
{
    public interface IQaRunTestManager : IDisposable
    {
        Instrument Instrument { get; }
        IObservable<string> TestStatus { get; }
        VolumeTestManager VolumeTestManager { get; set; }

        Task InitializeTest(InstrumentType instrumentType, ICommPort commPort, TestSettings testSettings,
            CancellationToken ct = new CancellationToken(), Client client = null, IObserver<string> statusObserver = null);

        Task RunCorrectionTest(int level, CancellationToken ct = new CancellationToken());
        Task RunVolumeTest(CancellationToken ct);
        Task SaveAsync();
        Task RunVerifiers();
    }

    public class QaRunTestManager : IQaRunTestManager
    {
        protected static Logger Log = LogManager.GetCurrentClassLogger();
        private EvcCommunicationClient _communicationClient;
        private readonly IEventAggregator _eventAggregator;
        private readonly TestRunService _testRunService;
        private readonly IReadingStabilizer _readingStabilizer;
        private readonly TachometerService _tachometerService;
        private readonly ISettingsService _settingsService;
        private readonly IEnumerable<IPreTestValidation> _validators;
        private readonly IEnumerable<IPostTestAction> _postTestCommands;

        public QaRunTestManager(
            IEventAggregator eventAggregator,
            TestRunService testRunService,
            IReadingStabilizer readingStabilizer,
            TachometerService tachometerService,
            ISettingsService settingsService,
            IEnumerable<IPreTestValidation> validators = null,
            IEnumerable<IPostTestAction> postTestCommands = null)
        {
            _eventAggregator = eventAggregator;
            _testRunService = testRunService;
            _readingStabilizer = readingStabilizer;
            _tachometerService = tachometerService;
            _settingsService = settingsService;
            _validators = validators;
            _postTestCommands = postTestCommands;
        }

        public VolumeTestManager VolumeTestManager { get; set; }

        private readonly Subject<string> _testStatus = new Subject<string>();
        public IObservable<string> TestStatus => _testStatus.AsObservable();

        public Instrument Instrument { get; private set; }

        public async Task InitializeTest(InstrumentType instrumentType, ICommPort commPort, TestSettings testSettings, 
            CancellationToken ct = new CancellationToken(), Client client = null, IObserver<string> statusObserver = null)
        {
            if (statusObserver != null)
                TestStatus.Subscribe(statusObserver);

            _communicationClient = instrumentType.ClientFactory.Invoke(commPort, _testStatus);
            ct.ThrowIfCancellationRequested();

            await _communicationClient.Connect(ct);
            ct.ThrowIfCancellationRequested();

            _testStatus.OnNext("Downloading items...");
            var items = await _communicationClient.GetAllItems();

            Instrument = Instrument.Create(instrumentType, items, testSettings, client);

            await RunVerifiers();
            await _communicationClient.Disconnect();

            if (Instrument.VolumeTest.DriveType is MechanicalDrive && _settingsService.Shared.TestSettings.MechanicalDriveVolumeTestType == TestSettings.VolumeTestType.Manual)
                VolumeTestManager = new ManualVolumeTestManager(_eventAggregator, _communicationClient, Instrument.VolumeTest, _settingsService);
            else
                VolumeTestManager = new AutoVolumeTestManager(_eventAggregator, _communicationClient, Instrument.VolumeTest, _tachometerService, _settingsService);                
        }    
       

        public async Task RunCorrectionTest(int level, CancellationToken ct)
        {
            if (Instrument == null)
                throw new NullReferenceException("Call InitializeTest before runnning a test");

            try
            {
                if (_settingsService.Shared.TestSettings.StabilizeLiveReadings)
                {
                    _testStatus.OnNext($"Stabilizing live readings...");
                    await _readingStabilizer.WaitForReadingsToStabilizeAsync(_communicationClient, Instrument, level, ct, _testStatus);
                }

                await DownloadVerificationTestItems(level, ct);
            }
            catch (OperationCanceledException)
            {
                Log.Info("Test run cancelled.");
            }
        }

        public async Task RunVolumeTest(CancellationToken ct)
        {
            try
            {
                if (Instrument.VerificationTests.Any(x => x.VolumeTest != null))
                {
                    VolumeTestManager.StatusMessage.Subscribe(_testStatus);
                    await VolumeTestManager.RunTest(ct);

                    //Execute any Post test clean up methods
                    foreach (var command in _postTestCommands)
                        await command.Execute(_communicationClient, Instrument, _testStatus);
                }
            }
            catch (OperationCanceledException)
            {
                _testStatus.OnNext($"Volume test cancelled.");
                Log.Info("Volume test cancelled.");
            }
        }

        public async Task SaveAsync()
        {
            try
            {
                _testStatus.OnNext($"Saving test...");

                await _testRunService.Save(Instrument);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public async Task RunVerifiers()
        {
            if (_validators != null && _validators.Any())
                foreach (var validator in _validators)
                {
                    _testStatus.OnNext($"Verifying items...");
                    await validator.Validate(_communicationClient, Instrument);
                }
        }

        public void Dispose()
        {
            _communicationClient?.Dispose();
            _tachometerService?.Dispose();
            VolumeTestManager?.Dispose();
        }

        private async Task DownloadVerificationTestItems(int level, CancellationToken ct)
        {
            await _communicationClient.Connect(ct);

            if (Instrument.CompositionType == EvcCorrectorType.PTZ)
            {
                _testStatus.OnNext($"Downloading P & T items...");
                await DownloadTemperatureTestItems(level);
                await DownloadPressureTestItems(level);
            }

            if (Instrument.CompositionType == EvcCorrectorType.T)
            {
                _testStatus.OnNext($"Downloading T items...");
                await DownloadTemperatureTestItems(level);
            }

            if (Instrument.CompositionType == EvcCorrectorType.P)
            {
                _testStatus.OnNext($"Downloading P items...");
                await DownloadPressureTestItems(level);
            }

            await _communicationClient.Disconnect();
        }

        public async Task DownloadTemperatureTestItems(int levelNumber)
        {
            var firstOrDefault = Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == levelNumber);
            var test = firstOrDefault?.TemperatureTest;

            if (test != null)
                test.Items =
                    await _communicationClient.GetTemperatureTestItems();
        }

        public async Task DownloadPressureTestItems(int level)
        {
            var firstOrDefault = Instrument.VerificationTests.FirstOrDefault(x => x.TestNumber == level);
            var test = firstOrDefault?.PressureTest;
            if (test != null)
                test.Items = await _communicationClient.GetPressureTestItems();                   
        }
    }
}