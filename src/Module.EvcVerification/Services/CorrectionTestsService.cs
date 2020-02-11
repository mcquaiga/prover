using System;
using System.Collections.Generic;
using System.Text;
using Core.GasCalculations;
using Devices.Core;
using Devices.Core.Interfaces;
using Devices.Core.Items.ItemGroups;
using Domain.Models.EvcVerifications;
using Domain.Models.EvcVerifications.CorrectionTests;

namespace Domain.Services
{
    public class CorrectionTestsService
    {
        public CorrectionTestsService()
        {

        }

        public void AddCorrectionTestPoint(EvcVerification evcVerification, CorrectionTestDefinition testDefinition)
        {
            var device = evcVerification.Device;

            var test = new CorrectionTestPoint(device, testDefinition.Level);

            var comp = device.ItemGroup<ISiteInformationItems>().CompositionType;

            if (comp == CompositionType.P || comp == CompositionType.PTZ)
                CreatePressure(device.ItemGroup<IPressureItems>(), testDefinition.PressureGaugePercent);

            if (comp == CompositionType.T || comp == CompositionType.PTZ)
                test.AddTemperature(device.ItemGroup<ITemperatureItems>(), testDefinition.TemperatureGauge);

            if (comp == CompositionType.PTZ) test.AddSuperFactor(device.ItemGroup<ISuperFactorItems>());

            if (testDefinition.IsVolumeTest)
                test.AddVolume(testDefinition.MechanicalDriveTestLimits);
        }

        //internal void AddFrequency()
        //{
        //    FrequencyTest = new FrequencyTest(this);
        //}

        internal PressureTest CreatePressure(IPressureItems pressureItems, decimal pressureGaugePercent)
        {
            var gauge = PressureCalculator.GetGaugePressure(pressureItems.Range, pressureGaugePercent);
            var pCalc = new PressureCalculator(pressureItems, gauge, null);

            return new PressureTest(pressureItems, pCalc.ActualValue, pCalc.ExpectedValue, pCalc.Atmospheric, pCalc.Gauge);
        }

        internal void AddSuperFactor(ISuperFactorItems superItems)
        {
            //SuperFactorTest = new SuperFactorTest(superItems, PressureTest, TemperatureTest);
        }

        internal void AddTemperature(ITemperatureItems tempItems, decimal temperatureGauge)
        {
            // TemperatureTest = new TemperatureTest(tempItems, temperatureGauge);
        }

        internal void AddVolume(List<CorrectionTestDefinition.MechanicalUncorrectedTestLimit> mechanicalDriveTestLimits)
        {
            //if (Device.Volume.DriveType is MechanicalDrive)
            //{
            //    VolumeTest = new MechanicalVolumeTest(Device.Volume, mechanicalDriveTestLimits);
            //}
            //else
            //{
            //    VolumeTest = new VolumeTest(Device.Volume);
            //}
        }
    }
}
