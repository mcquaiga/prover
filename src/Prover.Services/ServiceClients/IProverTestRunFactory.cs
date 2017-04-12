using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Prover.Domain.Instrument;
using Prover.Domain.Models.Prover;
using Prover.Shared.Enums;

namespace Prover.Services.ServiceClients
{
    public interface IProverTestRunFactory
    {
        Task<ProverTestRun> Create(IInstrumentFactory instrumentFactory);
    }

    public class ProverTestRunFactory : IProverTestRunFactory
    {
        public async Task<ProverTestRun> Create(IInstrumentFactory instrumentFactory)
        {
            var instrument = await instrumentFactory.Create();

            var testRun = new ProverTestRun()
            {
                Instrument = instrument,
                TestPoints = new List<ProverTestPoint>()
            };

            foreach (TestLevel testLevel in Enum.GetValues(typeof(TestLevel)))
            {
                var pressure = default(ProverPressureTestPoint);
                if (instrument.CorrectorType == EvcCorrectorType.P || instrument.CorrectorType == EvcCorrectorType.PTZ)
                    pressure = new ProverPressureTestPoint(instrument.PressureItems.ItemData);

                var temperature = default(ProverTemperatureTestPoint);
                if (instrument.CorrectorType == EvcCorrectorType.T || instrument.CorrectorType == EvcCorrectorType.PTZ)
                    temperature = new ProverTemperatureTestPoint(instrument.TemperatureItems.ItemData);

                var volume = default(ProverVolumeTestPoint);
                if (testLevel == TestLevel.Level1)
                    volume = new ProverVolumeTestPoint(instrument.VolumeItems.ItemData);

                testRun.TestPoints.Add(new ProverTestPoint(testLevel)
                {
                    Pressure = pressure,
                    Temperature = temperature,
                    Volume = volume
                });
            }

            return testRun;
        }
    }
}