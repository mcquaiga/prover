﻿using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Prover.CommProtocol.Common;
using Prover.CommProtocol.Common.Items;
using Prover.Core.Communication;
using Prover.Core.ExternalDevices.DInOutBoards;
using Prover.Core.Models.Instruments;

namespace Prover.Core.VerificationTests.VolumeVerification
{
    public sealed class AutoVolumeTestManagerBase : VolumeTestManagerBase
    {
        private readonly IDInOutBoard _outputBoard;
        private readonly TachometerService _tachometerCommunicator;

        public AutoVolumeTestManagerBase(IEventAggregator eventAggregator, TachometerService tachComm)
            : base(eventAggregator)
        {
            _tachometerCommunicator = tachComm;
            _outputBoard = DInOutBoardFactory.CreateBoard(0, 0, 0);
        }


        //protected override async Task ZeroInstrumentVolumeItems()
        //{
        //    await InstrumentCommunicator.Connect();
        //    await InstrumentCommunicator.SetItemValue(264, "20140867");
        //    await InstrumentCommunicator.SetItemValue(434, "0");
        //    await InstrumentCommunicator.SetItemValue(113, "0");
        //    await InstrumentCommunicator.SetItemValue(892, "0");
        //    await base.ZeroInstrumentVolumeItems();
        //}

        protected override async Task ExecuteSyncTest(EvcCommunicationClient commClient, VolumeTest volumeTest)
        {
            await Task.Run(async () =>
            {
                await commClient.Disconnect();
                Log.Info("Running volume sync test...");
                ResetPulseCounts(volumeTest);
                _outputBoard.StartMotor();
                do
                {
                    volumeTest.PulseACount += FirstPortAInputBoard.ReadInput();
                    volumeTest.PulseBCount += FirstPortBInputBoard.ReadInput();
                } while (volumeTest.UncPulseCount < 1);

                _outputBoard.StopMotor();
            });
        }

        protected override async Task PreTest(EvcCommunicationClient commClient, VolumeTest volumeTest,
            IEvcItemReset evcTestItemReset)
        {
            await commClient.Connect();
            if (evcTestItemReset != null) await evcTestItemReset.PreReset(commClient);
            volumeTest.Items = await commClient.GetItemValues(commClient.ItemDetails.VolumeItems());
            await commClient.Disconnect();

            if (_tachometerCommunicator != null)
                await _tachometerCommunicator?.ResetTach();

            ResetPulseCounts(volumeTest);
        }

        private static void ResetPulseCounts(VolumeTest volumeTest)
        {
            volumeTest.PulseACount = 0;
            volumeTest.PulseBCount = 0;
        }

        protected override async Task ExecutingTest(VolumeTest volumeTest)
        {
            await Task.Run(() =>
            {
                _outputBoard?.StartMotor();
                
                do
                {
                    //TODO: Raise events so the UI can respond
                    volumeTest.PulseACount += FirstPortAInputBoard.ReadInput();
                    volumeTest.PulseBCount += FirstPortBInputBoard.ReadInput();
                } while ((volumeTest.UncPulseCount < volumeTest.DriveType.MaxUncorrectedPulses()) && !RequestStopTest);

                _outputBoard?.StopMotor();
            });
        }

        protected override async Task PostTest(EvcCommunicationClient commClient, VolumeTest volumeTest,
            IEvcItemReset evcPostTestItemReset)
        {
            await Task.Run(async () =>
            {
                try
                {
                    await commClient.Connect();
                    volumeTest.AfterTestItems = await commClient.GetItemValues(commClient.ItemDetails.VolumeItems());
                    if (evcPostTestItemReset != null) await evcPostTestItemReset.PostReset(commClient);
                }
                finally
                {
                    await commClient.Disconnect();
                }

                await GetAppliedInput(volumeTest);
            });
        }

        private async Task GetAppliedInput(VolumeTest volumeTest)
        {
            if (_tachometerCommunicator == null) return;

            int? result = null;
            var tries = 0;
            do
            {
                try
                {
                    tries++;
                    Log.Error($"Reading tachometer .... Attempt {tries} of 10");
                    result = await _tachometerCommunicator.ReadTach();
                }
                catch (Exception ex)
                {
                    Log.Error($"An error occured communication with the tachometer: {ex}");

                }
            } while (!result.HasValue || tries < 10);
            
            Log.Debug($"Applied Input: {result.Value}");

            volumeTest.AppliedInput = result.Value;
        }
    }
}