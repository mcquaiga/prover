using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prover.CommProtocol.Common.Models.Instrument;
using Prover.Domain.Model.Instrument;
using Prover.Domain.Model.Verifications;
using Prover.Shared.Enums;

namespace Prover.Services.Verification
{
    public interface IVerificationRunFactory
    {
        Task<VerificationRun> Create(IInstrumentFactory instrumentFactory);
    }

    public class VerificationRunFactory : IVerificationRunFactory
    {
        public async Task<VerificationRun> Create(IInstrumentFactory instrumentFactory)
        {
            var instrument = await instrumentFactory.Create();

            var instrumentType = new EvcInstrument()
            {
                InstrumentFactory = instrument.InstrumentFactory.TypeIdentifier,
                InstrumentType = instrument.Name,
                Items = instrument.ItemData
            };

            var testRun = new VerificationRun()
            {
                Instrument = instrumentType,
                InstrumentId = instrumentType.Id,
                TestPoints = new List<VerificationRunTestPoint>()
            };
            CreateTestPoints(instrument, testRun);

            return testRun;
        }

        private static List<VerificationRunTestPoint> CreateTestPoints(IInstrument instrument, VerificationRun testRun)
        {
            var testPointList = new List<VerificationRunTestPoint>();
            foreach (TestLevel testLevel in Enum.GetValues(typeof(TestLevel)))
            {
                var pressure = default(PressureTest);
                if (instrument.CorrectorType == EvcCorrectorType.P || instrument.CorrectorType == EvcCorrectorType.PTZ)
                    pressure = new PressureTest();

                var temperature = default(TemperatureTest);
                if (instrument.CorrectorType == EvcCorrectorType.T || instrument.CorrectorType == EvcCorrectorType.PTZ)
                    temperature = new TemperatureTest();

                var volume = default(VolumeTest);
                if (testLevel == TestLevel.Level1)
                {
                    if (instrument.VolumeItems.DriveType == DriveTypeDescripter.Rotary)
                        volume = new RotaryVolumeTest();
                    else
                    {
                        volume = new MechanicalVolumeTest();
                    }
                }

                testPointList.Add(new VerificationRunTestPoint(testLevel)
                {
                    Pressure = pressure,
                    Temperature = temperature,
                    Volume = volume
                });
            }

            return testPointList;
        }
    }
}